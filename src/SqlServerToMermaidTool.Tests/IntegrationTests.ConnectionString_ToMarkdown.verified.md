```mermaid
erDiagram
  Company["**Company**"] {
    int Id pk
    nvarchar(200) Name
  }
  Employee["**Employee**"] {
    int Id pk
    nvarchar(100) FirstName
    int CompanyId
  }
  Company ||--o{ Employee : "FK_Employee_Company"
```