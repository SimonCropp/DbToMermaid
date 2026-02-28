```mermaid
erDiagram
  Company {
    int Id PK
    nvarchar Name
  }
  Employee {
    int Id PK
    nvarchar FirstName
    int CompanyId
  }
  Company ||--o{ Employee : "FK_Employee_Company"
```
