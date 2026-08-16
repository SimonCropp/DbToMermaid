
```mermaid
erDiagram
  Company["**Company**"] {
    int Id pk
    nvarchar(200) Name
    varchar(50)(nullable) TaxNumber
    varchar(30)(nullable) Phone
    varchar(255)(nullable) Email
    datetime2 CreatedAt
    datetime2(nullable) ModifiedAt
  }
  Customer["**Customer**"] {
    int Id pk
    nvarchar(100) FirstName
    nvarchar(100) LastName
    varchar(255) Email
    varchar(30)(nullable) Phone
    int(nullable) CompanyId
    datetime2 CreatedAt
    datetime2(nullable) ModifiedAt
  }
  Employee["**Employee**"] {
    int Id pk
    nvarchar(100) FirstName
    nvarchar(100) LastName
    varchar(255) Email
    varchar(30)(nullable) Phone
    date HireDate
    int CompanyId
    datetime2 CreatedAt
    datetime2(nullable) ModifiedAt
    int(nullable) ManagerId
  }
  Manager["**Manager**"] {
    int Id pk
    int EmployeeId
    nvarchar(100) Department
    tinyint Level
    date StartDate
    date(nullable) EndDate
  }
  Order["**Order**"] {
    int Id pk
    varchar(30) OrderNumber
    int CustomerId
    datetime2 OrderDate
    varchar(20) Status
    decimal(18,2) SubTotal
    decimal(18,2) Tax
    decimal(18,2) Total
    nvarchar(1000)(nullable) Notes
    datetime2 CreatedAt
    datetime2(nullable) ModifiedAt
  }
  OrderItem["**OrderItem**"] {
    int Id pk
    int OrderId
    int ProductId
    int Quantity
    decimal(18,2) UnitPrice
    decimal(18,2) Discount
    decimal(30,2)(nullable) LineTotal "computed"
  }
  Product["**Product**"] {
    int Id pk
    varchar(50) Sku
    nvarchar(200) Name
    nvarchar(max)(nullable) Description
    decimal(18,2) UnitPrice
    int StockQty
    bit IsActive
    datetime2 CreatedAt
    datetime2(nullable) ModifiedAt
  }
  Company ||--o{ Customer : "FK_Customer_Company"
  Company ||--o{ Employee : "FK_Employee_Company"
  Customer ||--o{ Order : "FK_Order_Customer"
  Employee ||--o{ Manager : "FK_Manager_Employee"
  Manager ||--o{ Employee : "FK_Employee_Manager"
  Order ||--o{ OrderItem : "FK_OrderItem_Order"
  Product ||--o{ OrderItem : "FK_OrderItem_Product"
```