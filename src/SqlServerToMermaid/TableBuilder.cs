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
                .Select(ApplyPrimaryKey)
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

    // A primary key column is never nullable, however the key was declared and whatever the column says
    Column ApplyPrimaryKey(Column column)
    {
        if (PrimaryKeys.Contains(column.Name))
        {
            return column with
            {
                IsNullable = false
            };
        }

        return column;
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
