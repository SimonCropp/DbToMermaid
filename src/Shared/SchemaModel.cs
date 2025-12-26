sealed record Database(string Name, IReadOnlyList<Table> Tables, IReadOnlyList<ForeignKey> ForeignKeys);

sealed record Table(string Schema, string Name, IReadOnlyList<Column> Columns, IReadOnlySet<string>? PrimaryKeys);

sealed record Column(int Ordinal, string Name, string Type, bool IsNullable);

sealed record ForeignKey(string Name, string ParentSchema, string ParentTable, string ReferencedSchema, string ReferencedTable);
