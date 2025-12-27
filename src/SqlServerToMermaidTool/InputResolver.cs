namespace SqlServerToMermaidTool;

public enum InputType
{
    ConnectionString,
    FilePath,
    RawSql
}

public static class InputResolver
{
    static readonly string[] ConnectionStringKeywords =
    [
        "Server=",
        "Data Source=",
        "Initial Catalog=",
        "Database=",
        "Integrated Security=",
        "User Id=",
        "Password=",
        "Trusted_Connection=",
        "MultipleActiveResultSets=",
        "Encrypt=",
        "TrustServerCertificate="
    ];

    public static InputType Resolve(string input)
    {
        if (LooksLikeConnectionString(input))
        {
            return InputType.ConnectionString;
        }

        if (File.Exists(input))
        {
            return InputType.FilePath;
        }

        return InputType.RawSql;
    }

    static bool LooksLikeConnectionString(string input) =>
        ConnectionStringKeywords.Any(keyword =>
            input.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
