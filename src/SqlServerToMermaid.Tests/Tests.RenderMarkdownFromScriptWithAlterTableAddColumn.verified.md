```mermaid
erDiagram
  Items["**Items**"] {
    int Id pk
    nvarchar(100) Name
  }
  NewTable["**NewTable**"] {
    int(nullable) Value
  }
```