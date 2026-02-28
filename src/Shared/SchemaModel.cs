sealed record Database(IReadOnlyList<Table> Tables, IReadOnlyList<ForeignKey> ForeignKeys);

sealed record Table(string Schema, string Name, IReadOnlyList<Column> Columns, IReadOnlySet<string>? PrimaryKeys, string? Comment = null);

sealed record Column(int Ordinal, string Name, string Type, bool IsNullable, bool Computed, string? Comment = null);

sealed record ForeignKey(string Name, string ParentSchema, string ParentTable, string ReferencedSchema, string ReferencedTable);
