public class ErrorHandlingTests
{
    [Test]
    public async Task ConnectionString_InvalidServer_ReturnsConnectionError()
    {
        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();
        var command = new RenderCommand
        {
            Input = "Server=invalid.server.that.does.not.exist;Database=TestDb;Integrated Security=true;Connect Timeout=1",
            Output = outputPath
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console));
    }

    [Test]
    public async Task ConnectionString_Timeout_ReturnsTimeoutError()
    {
        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();
        // Use a connection string that will timeout - non-routable IP address
        var command = new RenderCommand
        {
            Input = "Server=10.255.255.1;Database=TestDb;Integrated Security=true;Connect Timeout=1",
            Output = outputPath
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console));
    }

    [Test]
    public async Task OutputFile_Locked_ReturnsLockedFileError()
    {
        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();

        // Lock the file with exclusive access
        await using var lockingStream = new FileStream(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        var command = new RenderCommand
        {
            Input = "create table Test (Id int primary key)",
            Output = outputPath
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console));
    }

    [Test]
    public async Task SqlScript_InvalidSyntax_ReturnsParseError()
    {
        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();
        var command = new RenderCommand
        {
            Input = "create tabl Test (Id int primary key)", // Missing 'E' in table
            Output = outputPath
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console));
    }

    [Test]
    public async Task OutputFile_DirectoryNotFound_ReturnsDirectoryError()
    {
        var console = new FakeInMemoryConsole();
        var command = new RenderCommand
        {
            Input = "create table Test (Id int primary key)",
            Output = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nonexistent", "output.md")
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console))
            .ScrubInlineGuids();
    }

    [Test]
    public async Task OutputFile_InvalidExtension_ReturnsExtensionError()
    {
        using var outputPath = new TempFile(extension: ".txt");
        var console = new FakeInMemoryConsole();
        var command = new RenderCommand
        {
            Input = "create table Test (Id int primary key)",
            Output = outputPath
        };

        await ThrowsValueTask(() => command.ExecuteAsync(console));
    }
}
