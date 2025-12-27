public static class InputResolver
{
    static readonly string[] connectionStringKeywords =
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
        connectionStringKeywords.Any(keyword =>
            input.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
