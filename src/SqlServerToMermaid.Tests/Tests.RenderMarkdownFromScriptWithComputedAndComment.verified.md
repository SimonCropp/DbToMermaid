```mermaid
erDiagram
  Employee["**Employee**"] {
    int Id
    decimal(18,2) Salary
    decimal(18,2) Bonus
    unknown(nullable) TotalPay "computed: Sum of salary and bonus"
  }
```