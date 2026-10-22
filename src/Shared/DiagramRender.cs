static class DiagramRender
{
    public static async Task RenderMarkdown(TextWriter writer, Database database, bool namesOnly, Cancel cancel)
    {
        await writer.WriteLineAsync("```mermaid");
        await Render(writer, database, namesOnly, cancel);
        await writer.WriteAsync("```");
    }

    public static async Task<string> RenderMarkdown(Database database, bool namesOnly, Cancel cancel)
    {
        await using var writer = new StringWriter();
        await RenderMarkdown(writer, database, namesOnly, cancel);
        return writer.ToString();
    }

    public static async Task RenderMarkdownToFile(Database database, string path, bool namesOnly, Cancel cancel)
    {
        await using var writer = File.CreateText(path);
        await RenderMarkdown(writer, database, namesOnly, cancel);
    }

    public static async Task<string> Render(Database database, bool namesOnly, Cancel cancel)
    {
        await using var writer = new StringWriter();
        await Render(writer, database, namesOnly, cancel);
        return writer.ToString();
    }

    public static async Task RenderToFile(Database database, string path, bool namesOnly, Cancel cancel)
    {
        await using var writer = File.CreateText(path);
        await Render(writer, database, namesOnly, cancel);
    }

    public static async Task<string> RenderSvg(Database database, bool namesOnly, Cancel cancel)
    {
        var mermaid = await Render(database, namesOnly, cancel);
        return Mermaid.Render(mermaid);
    }

    public static async Task RenderSvgToFile(Database database, string path, bool namesOnly, Cancel cancel)
    {
        var svg = await RenderSvg(database, namesOnly, cancel);
        await File.WriteAllTextAsync(path, svg, cancel);
    }

    public static async Task<byte[]> RenderPng(Database database, bool namesOnly, Cancel cancel)
    {
        var mermaid = await Render(database, namesOnly, cancel);
        return ImageSharpRenderer.RenderPng(mermaid);
    }

    public static async Task RenderPngToFile(Database database, string path, bool namesOnly, Cancel cancel)
    {
        var png = await RenderPng(database, namesOnly, cancel);
        await File.WriteAllBytesAsync(path, png, cancel);
    }

    public static async Task Render(TextWriter writer, Database database, bool namesOnly, Cancel cancel)
    {
        await writer.WriteLineAsync("erDiagram");

        foreach (var table in database.Tables)
        {
            cancel.ThrowIfCancellationRequested();
            var tableId = ToMermaidId(table.Schema, table.Name);
            var entityName = ToEntityName(table.Schema, table.Name);
            var header = $"  {entityName}[\"**{tableId}**\"]";
            if (table.Comment is not null)
            {
                header = $"  {entityName}[\"**{tableId}**: {EscapeText(table.Comment)}\"]";
            }

            if (namesOnly)
            {
                await writer.WriteLineAsync(header);
                continue;
            }

            await writer.WriteLineAsync($"{header} {{");

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
        var referencedName = ToEntityName(foreignKey.ReferencedSchema, foreignKey.ReferencedTable);
        var parentName = ToEntityName(foreignKey.ParentSchema, foreignKey.ParentTable);
        return writer.WriteLineAsync($"  {referencedName} ||--o{{ {parentName} : \"{EscapeText(foreignKey.Name)}\"");
    }

    static Task RenderColumn(TextWriter writer, Column column, Table table, Cancel cancel)
    {
        cancel.ThrowIfCancellationRequested();

        string EscapeComment() => EscapeText(column.Comment!);

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

    // Always quoted, since Mermaid reads keywords case-insensitively and some (class, style, end, one, many, to)
    // are plausible table names. A bare `Class` would be parsed as a class statement.
    static string ToEntityName(string schema, string table) =>
        $"\"{ToMermaidId(schema, table)}\"";

    // Text inside a quoted string can not hold a double quote, and a line break ends the statement.
    static string EscapeText(string value) =>
        value
            .Replace("\"", "'")
            .ReplaceLineEndings(" ");
}
