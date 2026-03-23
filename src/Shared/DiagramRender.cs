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
            if (table.Comment is not null)
            {
                await writer.WriteLineAsync($"  {tableId}[\"**{tableId}**: {table.Comment.Replace("\"", "'")}\"] {{");
            }
            else
            {
                await writer.WriteLineAsync($"  {tableId}[\"**{tableId}**\"] {{");
            }

            foreach (var column in table.Columns)
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

    static Task RenderColumn(TextWriter writer, Column column, Table table, Cancel cancel)
    {
        cancel.ThrowIfCancellationRequested();

        string EscapeComment() => column.Comment!.Replace("\"", "'");

        var colId = ToMermaidIdentifier(column.Name);
        var type = column.IsNullable ? $"{column.Type}(nullable)" : column.Type;
        var isKey = table.PrimaryKeys is not null &&
                    table.PrimaryKeys.Contains(column.Name);

        if (isKey)
        {
            return RenderKey();
        }

        return RenderNonKey();

        Task RenderKey()
        {
            if ((column.Computed, column.Comment) is (true, not null))
            {
                return writer.WriteLineAsync($"    {type} {colId} pk \"computed: {EscapeComment()}\"");
            }

            if ((column.Computed, column.Comment) is (true, null))
            {
                return writer.WriteLineAsync($"    {type} {colId} pk \"computed\"");
            }

            if ((column.Computed, column.Comment) is (false, not null))
            {
                return writer.WriteLineAsync($"    {type} {colId} pk \"{EscapeComment()}\"");
            }

            return writer.WriteLineAsync($"    {type} {colId} pk");
        }

        Task RenderNonKey()
        {
            if ((column.Computed, column.Comment) is (true, not null))
            {
                return writer.WriteLineAsync($"    {type} {colId} \"computed: {EscapeComment()}\"");
            }

            if ((column.Computed, column.Comment) is (true, null))
            {
                return writer.WriteLineAsync($"    {type} {colId} \"computed\"");
            }

            if ((column.Computed, column.Comment) is (false, not null))
            {
                return writer.WriteLineAsync($"    {type} {colId} \"{EscapeComment()}\"");
            }

            return writer.WriteLineAsync($"    {type} {colId}");
        }
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
