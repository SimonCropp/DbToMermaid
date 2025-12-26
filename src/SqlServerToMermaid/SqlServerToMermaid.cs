namespace DbToMermaid;

public sealed class SqlServerToMermaid
{
    public static async Task<string> RenderMarkdown(SqlConnection connection, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        return await DiagramRender.RenderMarkdown(database, cancel);
    }

    public static async Task RenderMarkdownToFile(SqlConnection connection, string path, Cancel cancel = default)
    {
        var database = await SchemaReader.Read(connection, cancel);
        await DiagramRender.RenderMarkdownToFile(database, path, cancel);
    }
}
