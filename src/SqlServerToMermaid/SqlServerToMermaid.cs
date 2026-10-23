using System.Runtime.CompilerServices;

namespace DbToMermaid;

/// <summary>
/// Generates Mermaid ER diagrams from SQL Server database schemas.
/// </summary>
public static class SqlServerToMermaid
{
    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<string> RenderMarkdown(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        return await DiagramRender.RenderMarkdown(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> RenderMarkdown(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdown(connection, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<string> Render(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        return await DiagramRender.Render(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> Render(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        Render(connection, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL Server database to a TextWriter.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task Render(SqlConnection connection, TextWriter writer, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.Render(writer, database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL Server database to a TextWriter.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task Render(SqlConnection connection, TextWriter writer, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        Render(connection, writer, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL Server database to a TextWriter.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task RenderMarkdown(SqlConnection connection, TextWriter writer, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.RenderMarkdown(writer, database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL Server database to a TextWriter.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderMarkdown(SqlConnection connection, TextWriter writer, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdown(connection, writer, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task RenderMarkdownToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.RenderMarkdownToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderMarkdownToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdownToFile(connection, path, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task RenderToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.RenderToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderToFile(connection, path, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<string> RenderMarkdownFromScript(string script, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderMarkdown(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> RenderMarkdownFromScript(string script, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdownFromScript(script, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<string> RenderFromScript(string script, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.Render(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> RenderFromScript(string script, bool namesOnly = false, Cancel cancel = default) =>
        RenderFromScript(script, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string to a TextWriter.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderFromScript(string script, TextWriter writer, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.Render(writer, database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string to a TextWriter.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderFromScript(string script, TextWriter writer, bool namesOnly = false, Cancel cancel = default) =>
        RenderFromScript(script, writer, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string to a TextWriter.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderMarkdownFromScript(string script, TextWriter writer, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderMarkdown(writer, database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string to a TextWriter.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderMarkdownFromScript(string script, TextWriter writer, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdownFromScript(script, writer, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderMarkdownToFileFromScript(string script, string path, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderMarkdownToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderMarkdownToFileFromScript(string script, string path, bool namesOnly = false, Cancel cancel = default) =>
        RenderMarkdownToFileFromScript(script, path, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderToFileFromScript(string script, string path, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderToFileFromScript(string script, string path, bool namesOnly = false, Cancel cancel = default) =>
        RenderToFileFromScript(script, path, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the SVG.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<string> RenderSvg(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        return await DiagramRender.RenderSvg(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the SVG.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> RenderSvg(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderSvg(connection, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL Server database and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the SVG to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task RenderSvgToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.RenderSvgToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL Server database and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the SVG to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderSvgToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderSvgToFile(connection, path, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>The PNG bytes.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<byte[]> RenderPng(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        return await DiagramRender.RenderPng(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL Server database.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>The PNG bytes.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<byte[]> RenderPng(SqlConnection connection, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderPng(connection, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL Server database and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the PNG to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static async Task RenderPngToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, filter, cancel);
        await DiagramRender.RenderPngToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL Server database and writes it to a file.
    /// </summary>
    /// <param name="connection">The SQL Server connection to read the schema from.</param>
    /// <param name="path">The file path to write the PNG to.</param>
    /// <param name="filter">Optional predicate selecting the tables to include. Foreign keys to an excluded table are omitted.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderPngToFile(SqlConnection connection, string path, Func<SmoTable, bool>? filter = null, bool namesOnly = false, Cancel cancel = default) =>
        RenderPngToFile(connection, path, filter, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the SVG.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<string> RenderSvgFromScript(string script, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderSvg(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the SVG.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<string> RenderSvgFromScript(string script, bool namesOnly = false, Cancel cancel = default) =>
        RenderSvgFromScript(script, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the SVG to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderSvgToFileFromScript(string script, string path, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderSvgToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the SVG to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderSvgToFileFromScript(string script, string path, bool namesOnly = false, Cancel cancel = default) =>
        RenderSvgToFileFromScript(script, path, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>The PNG bytes.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<byte[]> RenderPngFromScript(string script, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderPng(database, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL script string.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>The PNG bytes.</returns>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task<byte[]> RenderPngFromScript(string script, bool namesOnly = false, Cancel cancel = default) =>
        RenderPngFromScript(script, namesOnly, true, cancel);

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the PNG to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="relationshipLabels">When false, relationships are rendered without the foreign key name as a label.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [OverloadResolutionPriority(1)]
    public static Task RenderPngToFileFromScript(string script, string path, bool namesOnly = false, bool relationshipLabels = true, Cancel cancel = default)
    {
        var database = ScriptParser.Parse(script);
        return DiagramRender.RenderPngToFile(database, path, namesOnly, relationshipLabels, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from a SQL script string and writes it to a file.
    /// </summary>
    /// <param name="script">The SQL script containing CREATE TABLE statements.</param>
    /// <param name="path">The file path to write the PNG to.</param>
    /// <param name="namesOnly">When true, only the table names are rendered, without columns.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    [Obsolete("Use the overload that takes relationshipLabels.")]
    public static Task RenderPngToFileFromScript(string script, string path, bool namesOnly = false, Cancel cancel = default) =>
        RenderPngToFileFromScript(script, path, namesOnly, true, cancel);
}
