```mermaid
erDiagram
  Child["**Child**"] {
    int Id
    int(nullable) ParentId
  }
  Parent["**Parent**"] {
    int Id
  }
  Parent ||--o{ Child : "FK_Child_Parent"
```
