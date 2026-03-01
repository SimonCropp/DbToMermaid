namespace DbToMermaid;

public class SqlParseException(IList<ParseError> errors) :
    Exception
{
    public IList<ParseError> Errors { get; } = errors;

    public override string ToString()
    {
        var messages = string.Join(Environment.NewLine, Errors.Select(_ => $"Line {_.Line}: {_.Message}"));
        return $"SQL parse errors:{Environment.NewLine}{messages}";
    }

    public override string Message => ToString();
}