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
        if (PrimaryKeys.Count == 0)
        {
            var columns = ApplyComments(Columns);
            return new(Schema, Name, columns, null, Comment);
        }

        var sorted = Columns
            .OrderBy(_ => !PrimaryKeys.Contains(_.Name))
            .ThenBy(_ => _.Ordinal)
            .ToList();
        var sortedColumns = ApplyComments(sorted);
        return new(Schema, Name, sortedColumns, PrimaryKeys, Comment);
    }

    List<Column> ApplyComments(List<Column> columns) =>
        columns
            .Select(_ =>
                ColumnComments.TryGetValue(_.Name, out var comment)
                    ? _ with { Comment = comment }
                    : _)
            .ToList();
}