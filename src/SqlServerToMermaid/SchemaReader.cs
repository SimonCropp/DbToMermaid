static class SchemaReader
{
    public static async Task<Database> Read(SqlConnection connection, Cancel cancel)
    {

        var shouldClose = false;
        if (connection.State != ConnectionState.Open)
        {
            shouldClose = true;
            await connection.OpenAsync(cancel);
        }

        var serverConnection = new ServerConnection
        {
            ConnectionString = connection.ConnectionString,
        };
        try
        {
        var server = new Server(serverConnection);
        var db = server.Databases[connection.Database];

        var tables = db.Tables
            .Where(_ => !_.IsSystemObject)
            .OrderBy(_ => _.Schema, StringComparer.Ordinal)
            .ThenBy(_ => _.Name, StringComparer.Ordinal)
            .Select(table =>
            {
                var primaryKeys = GetPrimaryKeys(table);
                var tableComment = table.ExtendedProperties["MS_Description"]?.Value?.ToString();

                var columns = table.Columns
                    .OrderBy(_ => primaryKeys?.Contains(_.Name) != true)
                    .ThenBy(_ => _.ID)
                    .Select(_ => new Column(
                        Ordinal: _.ID,
                        Name: _.Name,
                        Type: FormatType(_.DataType),
                        IsNullable: _.Nullable,
                        Computed: _.Computed,
                        Comment: _.ExtendedProperties["MS_Description"]?.Value?.ToString()))
                    .ToList();

                return new Table(table.Schema, table.Name, columns, primaryKeys, tableComment);
            })
            .ToList();

        var validTables = db.Tables
            .Where(_ => !_.IsSystemObject)
            .Select(_ => (_.Schema, _.Name))
            .ToHashSet();

        var foreignKeys = db.Tables
            .Where(_ => !_.IsSystemObject)
            .SelectMany(_ => _.ForeignKeys)
            .Where(_ => validTables.Contains((_.ReferencedTableSchema, _.ReferencedTable)))
            .Select(_ => new ForeignKey(
                Name: _.Name,
                ParentSchema: _.Parent.Schema,
                ParentTable: _.Parent.Name,
                ReferencedSchema: _.ReferencedTableSchema,
                ReferencedTable: _.ReferencedTable))
            .OrderBy(_ => _.ReferencedSchema, StringComparer.Ordinal)
            .ThenBy(_ => _.ReferencedTable, StringComparer.Ordinal)
            .ThenBy(_ => _.ParentSchema, StringComparer.Ordinal)
            .ThenBy(_ => _.ParentTable, StringComparer.Ordinal)
            .ThenBy(_ => _.Name, StringComparer.Ordinal)
            .ToList();

        if (shouldClose)
        {
            await connection.CloseAsync();
        }

        return new(tables, foreignKeys);
        }
        finally
        {
            serverConnection.Disconnect();
        }
    }

    static HashSet<string>? GetPrimaryKeys(Microsoft.SqlServer.Management.Smo.Table table)
    {
        var primaryIndexes = table.Indexes
            .FirstOrDefault(_ => _.IndexKeyType == IndexKeyType.DriPrimaryKey);

        return primaryIndexes?.IndexedColumns
            .Select(_ => _.Name)
            .ToHashSet(StringComparer.Ordinal);
    }

    public static string FormatType(DataType dataType)
    {
        // Mermaid ER diagram type tokens cannot contain whitespace, so no space after the comma in eg decimal(18,2)
        var token = dataType.SqlDataType.ToString();
        var isMax = token.EndsWith("Max", StringComparison.Ordinal);
        if (isMax)
        {
            token = token[..^3];
        }

        token = token.ToLowerInvariant();
        if (token is "userdefineddatatype" or "none")
        {
            return dataType.Name;
        }

        if (isMax)
        {
            return $"{token}(max)";
        }

        return token + FormatArguments(dataType);
    }

    static string FormatArguments(DataType dataType)
    {
        switch (dataType.SqlDataType)
        {
            case SqlDataType.Char:
            case SqlDataType.NChar:
            case SqlDataType.VarChar:
            case SqlDataType.NVarChar:
            case SqlDataType.Binary:
            case SqlDataType.VarBinary:
                if (dataType.MaximumLength > 0)
                {
                    return $"({dataType.MaximumLength})";
                }

                return "";
            case SqlDataType.Decimal:
            case SqlDataType.Numeric:
                if (dataType.NumericPrecision > 0)
                {
                    return $"({dataType.NumericPrecision},{dataType.NumericScale})";
                }

                return "";
            // float precision, and fractional seconds scale for datetime2/datetimeoffset/time, are
            // deliberately omitted. They are noise in a diagram
            default:
                return "";
        }
    }
}
