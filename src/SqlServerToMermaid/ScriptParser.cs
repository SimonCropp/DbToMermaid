using Microsoft.SqlServer.TransactSql.ScriptDom;

static class ScriptParser
{
    public static Database Parse(string script)
    {
        var parser = new TSql160Parser(initialQuotedIdentifiers: false);
        using var reader = new StringReader(script);
        var fragment = parser.Parse(reader, out var errors);

        if (errors.Count > 0)
        {
            var messages = string.Join(Environment.NewLine, errors.Select(e => $"Line {e.Line}: {e.Message}"));
            throw new InvalidOperationException($"SQL parse errors:{Environment.NewLine}{messages}");
        }

        var tables = new Dictionary<(string Schema, string Name), TableBuilder>();
        var foreignKeys = new List<ForeignKey>();

        foreach (var batch in ((TSqlScript)fragment).Batches)
        {
            foreach (var statement in batch.Statements)
            {
                ProcessStatement(statement, tables, foreignKeys);
            }
        }

        var tableList = tables.Values
            .OrderBy(t => t.Schema, StringComparer.Ordinal)
            .ThenBy(t => t.Name, StringComparer.Ordinal)
            .Select(t => t.Build())
            .ToList();

        var fkList = foreignKeys
            .OrderBy(fk => fk.ReferencedSchema, StringComparer.Ordinal)
            .ThenBy(fk => fk.ReferencedTable, StringComparer.Ordinal)
            .ThenBy(fk => fk.ParentSchema, StringComparer.Ordinal)
            .ThenBy(fk => fk.ParentTable, StringComparer.Ordinal)
            .ThenBy(fk => fk.Name, StringComparer.Ordinal)
            .ToList();

        return new(tableList, fkList);
    }

    static void ProcessStatement(TSqlStatement statement, Dictionary<(string Schema, string Name), TableBuilder> tables, List<ForeignKey> foreignKeys)
    {
        switch (statement)
        {
            case CreateTableStatement createTable:
                ProcessCreateTable(createTable, tables, foreignKeys);
                break;
            case AlterTableAddTableElementStatement alterAdd:
                ProcessAlterTableAdd(alterAdd, tables, foreignKeys);
                break;
            case ExecuteStatement exec:
                ProcessExecute(exec, tables);
                break;
        }
    }

    static void ProcessCreateTable(CreateTableStatement createTable, Dictionary<(string Schema, string Name), TableBuilder> tables, List<ForeignKey> foreignKeys)
    {
        var schemaName = createTable.SchemaObjectName.SchemaIdentifier?.Value ?? "dbo";
        var tableName = createTable.SchemaObjectName.BaseIdentifier.Value;
        var key = (schemaName, tableName);

        var builder = new TableBuilder(schemaName, tableName);
        tables[key] = builder;

        var ordinal = 0;
        foreach (var columnDef in createTable.Definition.ColumnDefinitions)
        {
            var column = BuildColumn(columnDef, ordinal++);
            builder.Columns.Add(column);
        }

        foreach (var constraint in createTable.Definition.TableConstraints)
        {
            ProcessConstraint(constraint, schemaName, tableName, builder, foreignKeys);
        }
    }

    static void ProcessAlterTableAdd(AlterTableAddTableElementStatement alterAdd, Dictionary<(string Schema, string Name), TableBuilder> tables, List<ForeignKey> foreignKeys)
    {
        var schemaName = alterAdd.SchemaObjectName.SchemaIdentifier?.Value ?? "dbo";
        var tableName = alterAdd.SchemaObjectName.BaseIdentifier.Value;
        var key = (schemaName, tableName);

        if (!tables.TryGetValue(key, out var builder))
        {
            builder = new(schemaName, tableName);
            tables[key] = builder;
        }

        foreach (var element in alterAdd.Definition.TableConstraints)
        {
            ProcessConstraint(element, schemaName, tableName, builder, foreignKeys);
        }

        foreach (var columnDef in alterAdd.Definition.ColumnDefinitions)
        {
            var column = BuildColumn(columnDef, builder.Columns.Count);
            builder.Columns.Add(column);
        }
    }

    static void ProcessConstraint(ConstraintDefinition constraint, string schemaName, string tableName, TableBuilder builder, List<ForeignKey> foreignKeys)
    {
        switch (constraint)
        {
            case UniqueConstraintDefinition { IsPrimaryKey: true } pk:
                foreach (var col in pk.Columns)
                {
                    builder.PrimaryKeys.Add(col.Column.MultiPartIdentifier.Identifiers.Last().Value);
                }
                break;
            case ForeignKeyConstraintDefinition fk:
                var fkName = fk.ConstraintIdentifier?.Value ?? $"FK_{tableName}_{fk.ReferenceTableName.BaseIdentifier.Value}";
                var refSchema = fk.ReferenceTableName.SchemaIdentifier?.Value ?? "dbo";
                var refTable = fk.ReferenceTableName.BaseIdentifier.Value;
                foreignKeys.Add(new(fkName, schemaName, tableName, refSchema, refTable));
                break;
        }
    }

    static void ProcessExecute(ExecuteStatement exec, Dictionary<(string Schema, string Name), TableBuilder> tables)
    {
        var spec = exec.ExecuteSpecification;
        if (spec.ExecutableEntity is not ExecutableProcedureReference procRef)
        {
            return;
        }

        var procName = procRef.ProcedureReference.ProcedureReference.Name;
        var name = procName.BaseIdentifier.Value;
        if (!name.Equals("sp_addextendedproperty", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var parameters = spec.ExecutableEntity is ExecutableProcedureReference epr
            ? epr.Parameters
            : [];

        string? propName = null, propValue = null, level1Type = null, level1Name = null, level2Type = null, level2Name = null;

        foreach (var param in parameters)
        {
            var paramName = param.Variable?.Name?.ToLowerInvariant();
            var paramValue = GetLiteralValue(param.ParameterValue);

            switch (paramName)
            {
                case "@name":
                    propName = paramValue;
                    break;
                case "@value":
                    propValue = paramValue;
                    break;
                case "@level1type":
                    level1Type = paramValue;
                    break;
                case "@level1name":
                    level1Name = paramValue;
                    break;
                case "@level2type":
                    level2Type = paramValue;
                    break;
                case "@level2name":
                    level2Name = paramValue;
                    break;
            }
        }

        if (propName is null ||
            !propName.Equals("MS_Description", StringComparison.OrdinalIgnoreCase) ||
            propValue is null ||
            level1Type is null ||
            !level1Type.Equals("TABLE", StringComparison.OrdinalIgnoreCase) ||
            level1Name is null)
        {
            return;
        }

        // Find table — try with dbo default schema
        var key = ("dbo", level1Name);
        if (!tables.TryGetValue(key, out var builder))
        {
            // Try all schemas
            builder = tables.Values.FirstOrDefault(t => t.Name.Equals(level1Name, StringComparison.OrdinalIgnoreCase));
            if (builder is null)
            {
                return;
            }
        }

        if (level2Type is not null &&
            level2Type.Equals("COLUMN", StringComparison.OrdinalIgnoreCase) &&
            level2Name is not null)
        {
            builder.ColumnComments[level2Name] = propValue;
        }
        else
        {
            builder.Comment = propValue;
        }
    }

    static string? GetLiteralValue(ScalarExpression? expression) =>
        expression switch
        {
            StringLiteral str => str.Value,
            IntegerLiteral intLit => intLit.Value,
            _ => null
        };

    static Column BuildColumn(ColumnDefinition columnDef, int ordinal)
    {
        var columnName = columnDef.ColumnIdentifier.Value;
        var dataType = FormatDataType(columnDef.DataType);
        var isNullable = IsNullable(columnDef);
        var isComputed = columnDef.ComputedColumnExpression is not null;

        return new(ordinal, columnName, dataType, isNullable, isComputed);
    }

    static string FormatDataType(DataTypeReference? dataType)
    {
        if (dataType is null)
        {
            return "unknown";
        }

        return dataType switch
        {
            SqlDataTypeReference sqlType => sqlType.SqlDataTypeOption.ToString().ToLowerInvariant(),
            UserDataTypeReference userType => userType.Name.BaseIdentifier.Value.ToLowerInvariant(),
            _ => "unknown"
        };
    }

    static bool IsNullable(ColumnDefinition columnDef)
    {
        // Check for explicit NOT NULL or NULL constraint
        foreach (var constraint in columnDef.Constraints)
        {
            if (constraint is NullableConstraintDefinition nullableConstraint)
            {
                return nullableConstraint.Nullable;
            }
        }

        // Check if column is part of PRIMARY KEY (implicitly NOT NULL)
        foreach (var constraint in columnDef.Constraints)
        {
            if (constraint is UniqueConstraintDefinition { IsPrimaryKey: true })
            {
                return false;
            }
        }

        // Default to nullable if not specified
        return true;
    }

    sealed class TableBuilder(string schema, string name)
    {
        public string Schema { get; } = schema;
        public string Name { get; } = name;
        public List<Column> Columns { get; } = [];
        public HashSet<string> PrimaryKeys { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string? Comment { get; set; }
        public Dictionary<string, string> ColumnComments { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Table Build()
        {
            var columns = Columns.Select(c =>
                ColumnComments.TryGetValue(c.Name, out var comment)
                    ? c with { Comment = comment }
                    : c
            ).ToList();
            return new(Schema, Name, columns, PrimaryKeys.Count > 0 ? PrimaryKeys : null, Comment);
        }
    }
}
