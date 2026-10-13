```mermaid
erDiagram
  Customers["**Customers**"] {
    int CustomerId pk
    nvarchar(50) Name
    datetime2 PeriodStart
    datetime2 PeriodEnd
  }
  Orders["**Orders**"] {
    int OrderId pk
    int CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```