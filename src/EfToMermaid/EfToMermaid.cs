namespace DbToMermaid;

/// <summary>
/// Generates Mermaid ER diagrams from Entity Framework Core models.
/// </summary>
public static class EfToMermaid
{
    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block from an Entity Framework Core model.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A Markdown string containing the Mermaid ER diagram.</returns>
    public static Task<string> RenderMarkdown(IModel model, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderMarkdown(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from an Entity Framework Core model.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the Mermaid ER diagram.</returns>
    public static Task<string> Render(IModel model, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.Render(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram from an Entity Framework Core model to a TextWriter.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="writer">The TextWriter to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task Render(IModel model, TextWriter writer, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.Render(writer, database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as a Markdown code block and writes it to a file.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderMarkdownToFile(IModel model, string path, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderMarkdownToFile(database, path, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram and writes it to a file.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="path">The file path to write the diagram to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderToFile(IModel model, string path, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderToFile(database, path, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG from an Entity Framework Core model.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>A string containing the SVG.</returns>
    public static Task<string> RenderSvg(IModel model, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderSvg(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as SVG and writes it to a file.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="path">The file path to write the SVG to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderSvgToFile(IModel model, string path, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderSvgToFile(database, path, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG from an Entity Framework Core model.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    /// <returns>The PNG bytes.</returns>
    public static Task<byte[]> RenderPng(IModel model, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderPng(database, cancel);
    }

    /// <summary>
    /// Renders a Mermaid ER diagram as PNG and writes it to a file.
    /// </summary>
    /// <param name="model">The Entity Framework Core model to render.</param>
    /// <param name="path">The file path to write the PNG to.</param>
    /// <param name="cancel">Optional cancellation token.</param>
    public static Task RenderPngToFile(IModel model, string path, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderPngToFile(database, path, cancel);
    }
}
