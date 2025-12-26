namespace DbToMermaid;

/// <summary>
/// Generates Mermaid ER diagrams from SQL Server database schemas.
/// </summary>
public sealed class SqlServerToMermaid
{
    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    public static async Task<string> RenderMarkdown(SqlConnection connection, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        return await DiagramRender.RenderMarkdown(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static async Task RenderMarkdownToFile(SqlConnection connection, string path, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        await DiagramRender.RenderMarkdownToFile(database, path, cancel);
    }
}
