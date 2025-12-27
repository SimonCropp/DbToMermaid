public class IntegrationTests
{
    static SqlInstance instance = new("SqlServerToMermaidTool", (SqlConnection _) => Task.CompletedTask);

    [Test]
    public async Task ConnectionString_ToMarkdown()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText =
                """
                CREATE TABLE Company
                (
                    Id   INT PRIMARY KEY,
                    Name NVARCHAR(200) NOT NULL
                );

                CREATE TABLE Employee
                (
                    Id        INT PRIMARY KEY,
                    FirstName NVARCHAR(100) NOT NULL,
                    CompanyId INT           NOT NULL,

                    CONSTRAINT FK_Employee_Company
                      FOREIGN KEY (CompanyId)
                      REFERENCES Company(Id)
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();
        var cmd = new RenderCommand
        {
            Input = database.Connection.ConnectionString,
            Output = outputPath
        };

        await cmd.ExecuteAsync(console);

        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, extension: "md");
    }

    [Test]
    public async Task ConnectionString_ToMermaid()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText =
                """
                CREATE TABLE Users
                (
                    Id   INT PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        using var outputPath = new TempFile(extension: ".mmd");
        var console = new FakeInMemoryConsole();
        var cmd = new RenderCommand
        {
            Input = database.Connection.ConnectionString,
            Output = outputPath
        };

        await cmd.ExecuteAsync(console);

        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, extension: "mmd");
    }

    [Test]
    public async Task SqlFile_ToMarkdown()
    {
        using var sqlPath = new TempFile(extension: ".sql");
        using var outputPath = new TempFile(extension: ".md");
        await File.WriteAllTextAsync(sqlPath,
            """
            CREATE TABLE Products
            (
                Id    INT PRIMARY KEY,
                Name  NVARCHAR(200) NOT NULL,
                Price DECIMAL(18,2) NOT NULL
            );
            """);

        var console = new FakeInMemoryConsole();
        var cmd = new RenderCommand
        {
            Input = sqlPath,
            Output = outputPath
        };

        await cmd.ExecuteAsync(console);

        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, extension: "md");
    }

    [Test]
    public async Task RawSql_ToMarkdown()
    {
        using var outputPath = new TempFile(extension: ".md");
        var console = new FakeInMemoryConsole();
        var cmd = new RenderCommand
        {
            Input = "CREATE TABLE Orders (Id INT PRIMARY KEY, Total DECIMAL(18,2) NOT NULL)",
            Output = outputPath
        };

        await cmd.ExecuteAsync(console);

        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, extension: "md");
    }

    [Test]
    public async Task CustomNewLine()
    {
        using var outputPath = new TempFile(extension: ".mmd");
        var console = new FakeInMemoryConsole();
        var cmd = new RenderCommand
        {
            Input = "CREATE TABLE Test (Id INT PRIMARY KEY)",
            Output = outputPath,
            NewLine = "\\n"
        };

        await cmd.ExecuteAsync(console);

        var bytes = await File.ReadAllBytesAsync(outputPath);
        var content = Encoding.UTF8.GetString(bytes);

        // Verify only LF line endings (no CR)
        await Assert.That(content).Contains("\n");
        await Assert.That(content).DoesNotContain("\r\n");
    }
}
