
```mermaid
erDiagram
  Customers {
    int CustomerId pk
    nvarchar Name
  }
  Orders {
    int OrderId pk
    int CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```
