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
                create table Company
                (
                    Id   int primary key,
                    Name nvarchar(200) not null
                );

                create table Employee
                (
                    Id        int primary key,
                    FirstName nvarchar(100) not null,
                    CompanyId int           not null,

                    constraint FK_Employee_Company
                      foreign key (CompanyId)
                      references Company(Id)
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
                create table Users
                (
                    Id   int primary key,
                    Name nvarchar(100) not null
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
            create table Products
            (
                Id    int primary key,
                Name  nvarchar(200) not null,
                Price decimal(18,2) not null
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
            Input = "create table Orders (Id int primary key, Total decimal(18,2) not null)",
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
            Input = "create table Test (Id int primary key)",
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
