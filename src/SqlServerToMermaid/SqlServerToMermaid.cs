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
    /// Renders a Mermaid ER diagram from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    public static async Task<string> Render(SqlConnection connection, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        return await DiagramRender.Render(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL Server database to a TextWriter.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static async Task Render(SqlConnection connection, TextWriter writer, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        await DiagramRender.Render(writer, database, cancel);
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

    /// <summary>
    /// Renders a Mermaid ER diagram and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static async Task RenderToFile(SqlConnection connection, string path, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        await DiagramRender.RenderToFile(database, path, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="databaseName">Optional name for the database in the diagram.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    public static Task<string> RenderMarkdownFromScript(string script, string databaseName = "Database", Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script, databaseName);
        return DiagramRender.RenderMarkdown(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="databaseName">Optional name for the database in the diagram.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    public static Task<string> RenderFromScript(string script, string databaseName = "Database", Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script, databaseName);
        return DiagramRender.Render(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="databaseName">Optional name for the database in the diagram.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderMarkdownToFileFromScript(string script, string path, string databaseName = "Database", Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script, databaseName);
        return DiagramRender.RenderMarkdownToFile(database, path, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="databaseName">Optional name for the database in the diagram.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderToFileFromScript(string script, string path, string databaseName = "Database", Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script, databaseName);
        return DiagramRender.RenderToFile(database, path, cancel);
    }
}
