```mermaid
erDiagram
  Child["**Child**"] {
    int Id pk
    int(nullable) ParentId
  }
  Parent["**Parent**"] {
    int Id pk
  }
  Parent ||--o{ Child : "FK_Child_Parent"
```