
```mermaid
erDiagram
  sales_Customers {
    int CustomerId(pk) "not null"
    nvarchar Name "not null"
  }
  sales_Orders {
    int CustomerId "not null"
    int OrderId(pk) "not null"
  }
  sales_Customers ||--o{ sales_Orders : "FK_Orders_Customers"
```
