namespace DbToMermaid;

public sealed class EfToMermaid
{
    public static Task<string> RenderMarkdown(IModel model, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderMarkdown(database, cancel);
    }

    public static Task RenderMarkdownToFile(IModel model, string path, Cancel cancel = default)
    {
        var database = SchemaReader.Read(model);
        return DiagramRender.RenderMarkdownToFile(database, path, cancel);
    }
}
