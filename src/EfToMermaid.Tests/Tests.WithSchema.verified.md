
```mermaid
erDiagram
  sales_Customers {
    int CustomerId PK
    nvarchar Name
  }
  sales_Orders {
    int OrderId PK
    int CustomerId
  }
  sales_Customers ||--o{ sales_Orders : "FK_Orders_Customers"
```
