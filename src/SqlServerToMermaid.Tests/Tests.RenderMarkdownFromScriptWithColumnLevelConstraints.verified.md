```mermaid
erDiagram
  Child["**Child**"] {
    int Id pk
    nvarchar(100) Name
    int ParentId
    int(nullable) OwnerId
  }
  Parent["**Parent**"] {
    int Id pk
    nvarchar(100) Name
  }
  Parent ||--o{ Child : "FK_Child_Owner"
  Parent ||--o{ Child : "fk_Child_Parent"
```