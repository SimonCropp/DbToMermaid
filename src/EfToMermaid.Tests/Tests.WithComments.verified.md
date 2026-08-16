
```mermaid
erDiagram
  Customers["**Customers**: Core customer information"] {
    int CustomerId pk "Auto-generated identifier"
    nvarchar(50) Name "Customer full name"
    nvarchar(max) ShippingAddress_City
    nvarchar(max) ShippingAddress_Street
  }
  Orders["**Orders**: Customer orders"] {
    int OrderId pk "Auto-generated identifier"
    int CustomerId
  }
  Customers ||--o{ Orders : "FK_Orders_Customers"
```