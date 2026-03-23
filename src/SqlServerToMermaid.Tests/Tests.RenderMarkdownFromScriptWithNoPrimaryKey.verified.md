```mermaid
erDiagram
  AuditLog["**AuditLog**"] {
    datetime2 Timestamp
    nvarchar Action
    nvarchar(nullable) Detail
  }
```
