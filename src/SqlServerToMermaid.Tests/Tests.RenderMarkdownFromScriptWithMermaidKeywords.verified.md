```mermaid
erDiagram
  "Class"["**Class**: A class spanning lines"] {
    int Id pk
    nvarchar(100) Name
  }
  "End"["**End**"] {
    int Id pk
    int ClassId
    int(nullable) StyleId
  }
  "Style"["**Style**"] {
    int Id pk
  }
  "Class" ||--o{ "End" : "FK_End_Class"
  "Style" ||--o{ "End" : "FK_End_Style"
```