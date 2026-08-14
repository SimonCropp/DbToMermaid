```mermaid
erDiagram
  Company["**Company**"] {
    int Id
    nvarchar(200) Name
    datetime2 CreatedAt
  }
  Employee["**Employee**"] {
    int Id
    nvarchar(100) FirstName
    nvarchar(100) LastName
    int CompanyId
    decimal(18,2) Salary
    decimal(18,2) Bonus
    unknown(nullable) TotalPay "computed"
  }
  Company ||--o{ Employee : "FK_Employee_Company"
```