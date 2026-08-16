
```mermaid
erDiagram
  Customers["**Customers**"] {
    int CustomerId pk
    nvarchar(50) Name
    nvarchar(50) ShippingAddress_City
    nvarchar(100) ShippingAddress_Street
  }
  Orders["**Orders**"] {
    int OrderId pk
    int CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```