namespace DbToMermaid;

/// <summary>
/// Thrown when a SQL script contains syntax errors that prevent parsing.
/// </summary>
/// <param name="errors">The parse errors reported by the SQL parser.</param>
public class SqlParseException(IList<ParseError> errors) :
    Exception
{
    /// <summary>
    /// The parse errors reported by the SQL parser.
    /// </summary>
    public IList<ParseError> Errors { get; } = errors;

    /// <inheritdoc />
    public override string ToString()
    {
        var messages = string.Join(Environment.NewLine, Errors.Select(_ => $"Line {_.Line}: {_.Message}"));
        return $"SQL parse errors:{Environment.NewLine}{messages}";
    }

    /// <inheritdoc />
    public override string Message => ToString();
}