public class InputResolverTests
{
    [Test]
    public async Task ConnectionString_WithServer()
    {
        var input = "Server=localhost;Database=MyDb;Integrated Security=true";
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.ConnectionString);
    }

    [Test]
    public async Task ConnectionString_WithDataSource()
    {
        var input = "Data Source=.;Initial Catalog=Test;User Id=sa;Password=xxx";
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.ConnectionString);
    }

    [Test]
    public async Task ConnectionString_WithTrustedConnection()
    {
        var input = "Server=myserver;Database=mydb;Trusted_Connection=True";
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.ConnectionString);
    }

    [Test]
    public async Task FilePath_ExistingFile()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var result = InputResolver.Resolve(tempFile);
            await Assert.That(result).IsEqualTo(InputType.FilePath);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task RawSql_CreateTable()
    {
        var input = "CREATE TABLE Test (Id INT PRIMARY KEY)";
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.RawSql);
    }

    [Test]
    public async Task RawSql_NonExistentPath()
    {
        var input = "nonexistent_file_that_does_not_exist.sql";
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.RawSql);
    }

    [Test]
    public async Task RawSql_MultiLineScript()
    {
        var input = """
            CREATE TABLE Company (
                Id INT PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL
            );
            """;
        var result = InputResolver.Resolve(input);
        await Assert.That(result).IsEqualTo(InputType.RawSql);
    }
}
