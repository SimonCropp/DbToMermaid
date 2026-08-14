```mermaid
erDiagram
  AuditLog["**AuditLog**"] {
    datetime2 Timestamp
    nvarchar(100) Action
    nvarchar(500)(nullable) Detail
  }
```