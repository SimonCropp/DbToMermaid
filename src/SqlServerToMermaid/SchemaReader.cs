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

        var serverConnection = new ServerConnection(connection);
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
                    .OrderBy(_ => _.ID)
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

        var foreignKeys = db.Tables
            .Where(_ => !_.IsSystemObject)
            .SelectMany(_ => _.ForeignKeys)
            .Where(_ =>
            {
                var referenced = db.Tables[_.ReferencedTable, _.ReferencedTableSchema];
                return referenced is not null && !referenced.IsSystemObject;
            })
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
        // Mermaid ER diagram type token must be a simple word (no parentheses); normalize SMO types accordingly.
        var token = dataType.SqlDataType.ToString();
        if (token.EndsWith("Max", StringComparison.Ordinal))
        {
            token = token[..^3];
        }

        token = token.ToLowerInvariant();
        return token is "userdefineddatatype" or "none" ? dataType.Name : token;
    }
}
