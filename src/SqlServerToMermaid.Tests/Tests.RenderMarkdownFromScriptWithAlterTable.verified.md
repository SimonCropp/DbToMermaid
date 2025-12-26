```mermaid
erDiagram
  Child {
    int Id "not null"
    int ParentId "null"
  }
  Parent {
    int Id "not null"
  }
  Parent ||--o{ Child : "FK_Child_Parent"
```
