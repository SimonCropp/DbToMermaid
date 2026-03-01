```mermaid
erDiagram
  Company {
    int Id pk
    nvarchar Name
  }
  Employee {
    int Id pk
    nvarchar FirstName
    int CompanyId
  }
  Company ||--o{ Employee : "FK_Employee_Company"
```
