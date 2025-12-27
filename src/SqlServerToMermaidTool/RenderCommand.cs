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

        try
        {
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
        catch (SqlException ex) when (IsTimeoutError(ex))
        {
            throw new CommandException($"Database operation timed out: {ex.Message}");
        }
        catch (SqlException ex)
        {
            throw new CommandException($"Database connection failed: {ex.Message}");
        }
        catch (DirectoryNotFoundException)
        {
            var directory = Path.GetDirectoryName(fullPath);
            throw new CommandException($"Output directory does not exist: {directory}");
        }
        catch (IOException ex) when (IsFileLocked(ex))
        {
            throw new CommandException($"Output file is locked by another process: {fullPath}");
        }
        catch (IOException ex)
        {
            throw new CommandException($"File I/O error: {ex.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            throw new CommandException($"Permission denied writing to: {fullPath}");
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("SQL parse errors"))
        {
            throw new CommandException($"Invalid SQL schema:\n{ex.Message}");
        }
    }

    static bool IsTimeoutError(SqlException ex) =>
        ex.Number == -2 || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase);

    static bool IsFileLocked(IOException ex) =>
        ex.HResult == unchecked((int)0x80070020) || // ERROR_SHARING_VIOLATION
        ex.HResult == unchecked((int)0x80070021);   // ERROR_LOCK_VIOLATION

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
