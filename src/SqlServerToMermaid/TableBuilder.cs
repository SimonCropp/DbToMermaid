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
        var pks = PrimaryKeys.Count > 0 ? PrimaryKeys : null;
        var columns = Columns
            .OrderBy(_ => pks?.Contains(_.Name) != true)
            .ThenBy(_ => _.Ordinal)
            .Select(_ =>
                ColumnComments.TryGetValue(_.Name, out var comment)
                    ? _ with { Comment = comment }
                    : _)
            .ToList();
        return new(Schema, Name, columns, pks, Comment);
    }
}