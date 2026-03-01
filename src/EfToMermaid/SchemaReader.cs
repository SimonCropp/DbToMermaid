static class SchemaReader
{
    public static Database Read(IModel model)
    {
        var tableGroups = model.GetEntityTypes()
            .Where(_ => !_.IsOwned() && _.GetTableName() is not null)
            .GroupBy(_ => (Schema: _.GetSchema(), Table: _.GetTableName()!))
            .OrderBy(_ => _.Key.Schema ?? "dbo", StringComparer.Ordinal)
            .ThenBy(_ => _.Key.Table, StringComparer.Ordinal)
            .ToList();

        var tables = new List<Table>(tableGroups.Count);

        foreach (var group in tableGroups)
        {
            var schema = group.Key.Schema ?? "dbo";
            var tableName = group.Key.Table;
            var storeObject = StoreObjectIdentifier.Table(tableName, group.Key.Schema);
            var tableComment = group.First().FindAnnotation("Relational:Comment")?.Value?.ToString();

            var properties = group
                .SelectMany(_ => _.GetProperties())
                .DistinctBy(_ => _.Name)
                .Select(_ => BuildColumn(_, storeObject))
                .OrderBy(_ => _.Name, StringComparer.Ordinal)
                .Select((c, i) => c with { Ordinal = i })
                .ToList();

            var pkCols = group
                .SelectMany(_ => _.FindPrimaryKey()?.Properties ?? [])
                .Select(_ => _.GetColumnName(storeObject) ?? _.Name)
                .ToHashSet(StringComparer.Ordinal);

            tables.Add(new(schema, tableName, properties, pkCols, tableComment));
        }

        var foreignKeys = model.GetEntityTypes()
            .Where(_ => !_.IsOwned() && _.GetTableName() is not null)
            .SelectMany(_ => _.GetForeignKeys())
            .Select(_ =>
            {
                var depSchema = _.DeclaringEntityType.GetSchema() ?? "dbo";
                var depTable = _.DeclaringEntityType.GetTableName()!;
                var principalSchema = _.PrincipalEntityType.GetSchema() ?? "dbo";
                var principalTable = _.PrincipalEntityType.GetTableName()!;
                var name = _.GetConstraintName() ?? $"fk_{depTable}_{principalTable}";
                return new ForeignKey(name, depSchema, depTable, principalSchema, principalTable);
            })
            .OrderBy(_ => _.ReferencedSchema, StringComparer.Ordinal)
            .ThenBy(_ => _.ReferencedTable, StringComparer.Ordinal)
            .ThenBy(_ => _.ParentSchema, StringComparer.Ordinal)
            .ThenBy(_ => _.ParentTable, StringComparer.Ordinal)
            .ThenBy(_ => _.Name, StringComparer.Ordinal)
            .ToList();

        return new(tables, foreignKeys);
    }

    private static Column BuildColumn(IProperty property, StoreObjectIdentifier storeObject)
    {
        var name = property.GetColumnName(storeObject) ?? property.Name;
        var storeType = property.GetColumnType(storeObject);
        var type = FormatType(storeType, property.ClrType);
        var isComputed = property.GetComputedColumnSql(storeObject) is not null;
        var comment = property.FindAnnotation("Relational:Comment")?.Value?.ToString();
        return new(0, name, type, property.IsNullable, isComputed, comment);
    }

    static string FormatType(string? storeType, Type clrType)
    {
        var token = storeType;
        if (string.IsNullOrWhiteSpace(token))
        {
            token = clrType.Name;
        }

        token = token.Trim();
        var paren = token.IndexOf('(');
        if (paren >= 0)
        {
            token = token[..paren];
        }

        var space = token.IndexOf(' ');
        if (space >= 0)
        {
            token = token[..space];
        }

        return token.ToLowerInvariant();
    }
}
