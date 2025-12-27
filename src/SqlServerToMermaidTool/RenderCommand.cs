[Command(Description = "Generate Mermaid ER diagram from SQL Server database or script")]
public class RenderCommand : ICommand
{
    [CommandParameter(
        0,
        Name = "input",
        Description = "SQL connection string, path to .sql file, or raw SQL script")]
    public required string Input { get; init; }

    [CommandOption(
        "output",
        'o',
        Description = "Output file path (.md or .mmd). Default: schema.md")]
    public string Output { get; init; } = "schema.md";

    [CommandOption(
        "newline",
        'n',
        Description = @"Custom newline sequence (e.g., \n or \r\n)")]
    public string? NewLine { get; init; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var useMarkdown = ValidateAndGetOutputFormat(Output);
        var inputType = InputResolver.Resolve(Input);

        var fullPath = Path.GetFullPath(Output);
        await using var writer = new StreamWriter(fullPath);

        if (NewLine is not null)
        {
            writer.NewLine = ParseNewLine(NewLine);
        }

        var task = (inputType, useMarkdown) switch
        {
            (InputType.ConnectionString, true) => RenderConnectionMarkdown(writer),
            (InputType.ConnectionString, false) => RenderConnectionRaw(writer),
            (InputType.FilePath, true) => RenderFileMarkdown(writer),
            (InputType.FilePath, false) => RenderFileRaw(writer),
            (InputType.RawSql, true) => RenderScriptMarkdown(writer, Input),
            (InputType.RawSql, false) => RenderScriptRaw(writer, Input),
            _ => throw new InvalidOperationException("Unexpected input/output combination")
        };
        await task;

        await console.Output.WriteLineAsync($"Generated: {fullPath}");
    }

    static bool ValidateAndGetOutputFormat(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".md" => true,
            ".mmd" => false,
            _ => throw new CommandException(
                $"Invalid output extension '{extension}'. Only .md and .mmd are supported.")
        };
    }

    static string ParseNewLine(string newLine) =>
        newLine
            .Replace("\\r", "\r")
            .Replace("\\n", "\n");

    async Task RenderConnectionMarkdown(TextWriter writer)
    {
        await using var connection = new SqlConnection(Input);
        await connection.OpenAsync();
        await SqlServerToMermaid.RenderMarkdown(connection, writer);
    }

    async Task RenderConnectionRaw(TextWriter writer)
    {
        await using var connection = new SqlConnection(Input);
        await connection.OpenAsync();
        await SqlServerToMermaid.Render(connection, writer);
    }

    async Task RenderFileMarkdown(TextWriter writer)
    {
        var script = await File.ReadAllTextAsync(Input);
        await SqlServerToMermaid.RenderMarkdownFromScript(script, writer);
    }

    async Task RenderFileRaw(TextWriter writer)
    {
        var script = await File.ReadAllTextAsync(Input);
        await SqlServerToMermaid.RenderFromScript(script, writer);
    }

    static Task RenderScriptMarkdown(TextWriter writer, string script) =>
        SqlServerToMermaid.RenderMarkdownFromScript(script, writer);

    static Task RenderScriptRaw(TextWriter writer, string script) =>
        SqlServerToMermaid.RenderFromScript(script, writer);
}
