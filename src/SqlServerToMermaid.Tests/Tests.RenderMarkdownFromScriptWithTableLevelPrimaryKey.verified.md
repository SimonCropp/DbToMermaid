```mermaid
erDiagram
  "Child"["**Child**"] {
    int Id pk
    int ParentId
  }
  "Parent"["**Parent**"] {
    int Id pk
    emailaddress(nullable) Email
  }
  "Parent" ||--o{ "Child" : "fk_Child_Parent"
```