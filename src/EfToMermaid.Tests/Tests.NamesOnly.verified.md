
```mermaid
erDiagram
  "Customers"["**Customers**"]
  "Orders"["**Orders**"]
  "Customers" ||--o{ "Orders" : "FK_Orders_Customers"
```