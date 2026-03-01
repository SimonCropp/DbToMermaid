```mermaid
erDiagram
  sales_Customers {
    int CustomerId pk
    nvarchar Name
  }
  sales_Orders {
    int OrderId pk
    int CustomerId
  }
  sales_Customers ||--o{ sales_Orders : "FK_Orders_Customers"
```
