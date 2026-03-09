
```mermaid
erDiagram
  Customers["**Customers**"] {
    int CustomerId pk
    nvarchar Name
    nvarchar ShippingAddress_City
    nvarchar ShippingAddress_Street
  }
  Orders["**Orders**"] {
    int OrderId pk
    int CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```
