```mermaid
erDiagram
  Employee["**Employee**"] {
    int Id
    decimal Salary
    decimal Bonus
    unknown(nullable) TotalPay "computed: Sum of salary and bonus"
  }
```
