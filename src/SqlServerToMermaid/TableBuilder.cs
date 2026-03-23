class TableBuilder(string schema, string name)
{
    public string Schema { get; } = schema;
    public string Name { get; } = name;
    public List<Column> Columns { get; } = [];
    public HashSet<string> PrimaryKeys { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string? Comment { get; set; }
    public Dictionary<string, string> ColumnComments { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Table Build()
    {
        if (PrimaryKeys.Count > 0)
        {
            var columns = Columns
                .OrderBy(_ => !PrimaryKeys.Contains(_.Name))
                .ThenBy(_ => _.Ordinal)
                .Select(ApplyComment)
                .ToList();
            return new(Schema, Name, columns, PrimaryKeys, Comment);
        }
        else
        {
            var columns = Columns
                .Select(ApplyComment)
                .ToList();
            return new(Schema, Name, columns, null, Comment);
        }
    }

    Column ApplyComment(Column column)
    {
        if (ColumnComments.TryGetValue(column.Name, out var comment))
        {
            return column with
            {
                Comment = comment
            };
        }

        return column;
    }
}
