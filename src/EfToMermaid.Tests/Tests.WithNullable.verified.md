
```mermaid
erDiagram
  Customers {
    int CustomerId PK
    nvarchar(nullable) Name
  }
  Orders {
    int OrderId PK
    int(nullable) CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```
