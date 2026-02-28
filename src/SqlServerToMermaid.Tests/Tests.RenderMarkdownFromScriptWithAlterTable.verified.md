```mermaid
erDiagram
  Child {
    int Id
    int(nullable) ParentId
  }
  Parent {
    int Id
  }
  Parent ||--o{ Child : "FK_Child_Parent"
```
