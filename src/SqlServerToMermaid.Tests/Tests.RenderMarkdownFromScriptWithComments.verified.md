```mermaid
erDiagram
  Customers["**Customers**: Core customer information"] {
    int CustomerId "Auto-generated identifier"
    nvarchar Name "Customer full name"
    varchar(nullable) Email
  }
```
