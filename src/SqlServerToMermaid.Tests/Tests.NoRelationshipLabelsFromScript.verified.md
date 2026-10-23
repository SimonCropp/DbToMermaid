```mermaid
erDiagram
  "Customers"["**Customers**"] {
    int CustomerId pk
  }
  "Orders"["**Orders**"] {
    int OrderId pk
    int CustomerId
  }
  "Customers" ||--o{ "Orders" : ""
```