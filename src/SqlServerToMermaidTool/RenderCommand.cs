[Command(Description = "Generate Mermaid ER diagram from SQL Server database or script")]
public partial class RenderCommand : ICommand
{
    [CommandParameter(
        0,
        Name = "input",
        Description = "SQL connection string, path to .sql file, or raw SQL script")]
    public required string Input { get; set; }

    [CommandOption(
        "output",
        'o',
        Description = "Output file path (.md, .mmd, .svg or .png). Default: schema.md")]
    public string Output { get; set; } = "schema.md";

    [CommandOption(
        "newline",
        'n',
        Description = @"Custom newline sequence (e.g., \n or \r\n). Applies to .md and .mmd output")]
    public string? NewLine { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var format = ValidateAndGetOutputFormat(Output);
        var inputType = InputResolver.Resolve(Input);
        var fullPath = Path.GetFullPath(Output);

        try
        {
            var task = format switch
            {
                OutputFormat.Svg => RenderSvg(fullPath, inputType),
                OutputFormat.Png => RenderPng(fullPath, inputType),
                _ => RenderText(fullPath, inputType, format == OutputFormat.Markdown)
            };
            await task;

            await console.Output.WriteLineAsync($"Generated: {fullPath}");
        }
        catch (SqlException exception)
            when (IsTimeoutError(exception))
        {
            throw new CommandException($"Database operation timed out: {exception.Message}");
        }
        catch (SqlException exception)
        {
            throw new CommandException($"Database connection failed: {exception.Message}");
        }
        catch (DirectoryNotFoundException)
        {
            var directory = Path.GetDirectoryName(fullPath);
            throw new CommandException($"Output directory does not exist: {directory}");
        }
        catch (IOException exception) when (IsFileLocked(exception))
        {
            throw new CommandException($"Output file is locked by another process: {fullPath}");
        }
        catch (IOException exception)
        {
            throw new CommandException($"File I/O error: {exception.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            throw new CommandException($"Permission denied writing to: {fullPath}");
        }
        catch (SqlParseException exception)
        {
            throw new CommandException(exception.Message);
        }
        catch (MermaidException exception)
        {
            throw new CommandException($"Failed to render diagram: {exception.Message}");
        }
    }

    static bool IsTimeoutError(SqlException exception) =>
        exception.Number == -2 ||
        exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase);

    static bool IsFileLocked(IOException exception) =>
        // ERROR_SHARING_VIOLATION and ERROR_LOCK_VIOLATION
        exception.HResult is unchecked((int)0x80070020) or unchecked((int)0x80070021);

    static OutputFormat ValidateAndGetOutputFormat(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".md" => OutputFormat.Markdown,
            ".mmd" => OutputFormat.Mermaid,
            ".svg" => OutputFormat.Svg,
            ".png" => OutputFormat.Png,
            _ => throw new CommandException(
                $"Invalid output extension '{extension}'. Only .md, .mmd, .svg and .png are supported.")
        };
    }

    static string ParseNewLine(string newLine) =>
        newLine
            .Replace("\\r", "\r")
            .Replace("\\n", "\n");

    async Task RenderText(string path, InputType inputType, bool useMarkdown)
    {
        await using var writer = new StreamWriter(path);

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
            _ => throw new("Unexpected input/output combination")
        };
        await task;
    }

    Task RenderSvg(string path, InputType inputType) =>
        inputType switch
        {
            InputType.ConnectionString => RenderConnectionSvg(path),
            InputType.FilePath => RenderFileSvg(path),
            InputType.RawSql => SqlServerToMermaid.RenderSvgToFileFromScript(Input, path),
            _ => throw new("Unexpected input type")
        };

    Task RenderPng(string path, InputType inputType) =>
        inputType switch
        {
            InputType.ConnectionString => RenderConnectionPng(path),
            InputType.FilePath => RenderFilePng(path),
            InputType.RawSql => SqlServerToMermaid.RenderPngToFileFromScript(Input, path),
            _ => throw new("Unexpected input type")
        };

    async Task RenderConnectionSvg(string path)
    {
        await using var connection = new SqlConnection(Input);
        await connection.OpenAsync();
        await SqlServerToMermaid.RenderSvgToFile(connection, path);
    }

    async Task RenderConnectionPng(string path)
    {
        await using var connection = new SqlConnection(Input);
        await connection.OpenAsync();
        await SqlServerToMermaid.RenderPngToFile(connection, path);
    }

    async Task RenderFileSvg(string path)
    {
        var script = await File.ReadAllTextAsync(Input);
        await SqlServerToMermaid.RenderSvgToFileFromScript(script, path);
    }

    async Task RenderFilePng(string path)
    {
        var script = await File.ReadAllTextAsync(Input);
        await SqlServerToMermaid.RenderPngToFileFromScript(script, path);
    }

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
