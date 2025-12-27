static class DiagramRender
{
    public static async Task RenderMarkdown(TextWriter writer, Database database, Cancel cancel)
    {
        await writer.WriteLineAsync("```mermaid");
        await Render(writer, database, cancel);
        await writer.WriteAsync("```");
    }

    public static async Task<string> RenderMarkdown(Database database, Cancel cancel)
    {
        await using var writer = new StringWriter();
        await RenderMarkdown(writer, database, cancel);
        return writer.ToString();
    }

    public static async Task RenderMarkdownToFile(Database database, string path, Cancel cancel)
    {
        await using var writer = File.CreateText(path);
        await RenderMarkdown(writer, database, cancel);
    }

    public static async Task<string> Render(Database database, Cancel cancel)
    {
        await using var writer = new StringWriter();
        await Render(writer, database, cancel);
        return writer.ToString();
    }

    public static async Task RenderToFile(Database database, string path, Cancel cancel)
    {
        await using var writer = File.CreateText(path);
        await Render(writer, database, cancel);
    }

    public static async Task Render(TextWriter writer, Database database, Cancel cancel)
    {
        await writer.WriteLineAsync("erDiagram");

        foreach (var table in database.Tables)
        {
            cancel.ThrowIfCancellationRequested();
            var tableId = ToMermaidId(table.Schema, table.Name);
            await writer.WriteLineAsync($"  {tableId} {{");

            foreach (var column in table.Columns
                .OrderBy(_ => table.PrimaryKeys?.Contains(_.Name) != true)
                .ThenBy(_ => _.Ordinal))
            {
                await RenderColumn(writer, column, table, cancel);
            }

            await writer.WriteLineAsync("  }");
        }

        foreach (var foreignKey in database.ForeignKeys)
        {
            await RenderForeignKey(writer, foreignKey, cancel);
        }
    }

    static Task RenderForeignKey(TextWriter writer, ForeignKey foreignKey, Cancel cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var referencedId = ToMermaidId(foreignKey.ReferencedSchema, foreignKey.ReferencedTable);
        var parentId = ToMermaidId(foreignKey.ParentSchema, foreignKey.ParentTable);
        return writer.WriteLineAsync($"  {referencedId} ||--o{{ {parentId} : \"{foreignKey.Name}\"");
    }

    static async Task RenderColumn(TextWriter writer, Column column, Table table, Cancel cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var colId = ToMermaidIdentifier(column.Name);
        var isPrimaryKey = table.PrimaryKeys != null && table.PrimaryKeys.Contains(column.Name);

        await writer.WriteAsync("    ");
        await writer.WriteAsync(column.Type);
        await writer.WriteAsync(' ');
        await writer.WriteAsync(isPrimaryKey ? $"{colId}(pk)" : colId);

        await writer.WriteAsync(" \"");
        await writer.WriteAsync(column.IsNullable ? "null" : "not null");
        if (column.Computed)
        {
            await writer.WriteAsync(", computed");
        }
        await writer.WriteAsync('"');
        await writer.WriteLineAsync();
    }

    static string ToMermaidIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "_";
        }

        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            builder.Append(char.IsLetterOrDigit(ch) ? ch : '_');
        }

        if (char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }

        return builder.ToString();
    }

    static string ToMermaidId(string schema, string table)
    {
        if (schema == "dbo")
        {
            return ToMermaidIdentifier(table);
        }

        return ToMermaidIdentifier($"{schema}_{table}");
    }
}
