
```mermaid
erDiagram
  %% Core customer information
  Customers {
    int CustomerId(pk) "not null: Auto-generated identifier"
    nvarchar Name "not null: Customer full name"
  }
  %% Customer orders
  Orders {
    int OrderId(pk) "not null: Auto-generated identifier"
    int CustomerId "not null"
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```
