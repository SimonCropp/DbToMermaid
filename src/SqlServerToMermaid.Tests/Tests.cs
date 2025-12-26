// ReSharper disable UnusedVariable
public class Tests
{
    static SqlInstance instance = new("SqlServerToMermaid", (SqlConnection _) => Task.CompletedTask);

    static async Task Usage(SqlConnection sqlConnection)
    {
        #region SqlServerUsage

        var markdown = await SqlServerToMermaid.RenderMarkdown(sqlConnection);

        #endregion

        #region SqlServerUsageFile

        await SqlServerToMermaid.RenderMarkdownToFile(sqlConnection, "diagram.md");

        #endregion
    }

    [Test]
    public async Task RenderMarkdown()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText =
                """
                -- begin-snippet: SampleSchema

                CREATE TABLE Company
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    Name        NVARCHAR(200)   NOT NULL,
                    TaxNumber   VARCHAR(50)     NULL,
                    Phone       VARCHAR(30)     NULL,
                    Email       VARCHAR(255)    NULL,
                    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    ModifiedAt  DATETIME2       NULL
                );

                CREATE TABLE Address
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    Street      NVARCHAR(200)   NOT NULL,
                    City        NVARCHAR(100)   NOT NULL,
                    State       NVARCHAR(100)   NULL,
                    PostCode    VARCHAR(20)     NULL,
                    Country     NVARCHAR(100)   NOT NULL,
                    AddressType VARCHAR(20)     NOT NULL DEFAULT 'Billing',
                    CompanyId   INT             NULL,
                    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

                    CONSTRAINT FK_Address_Company FOREIGN KEY (CompanyId) REFERENCES Company(Id)
                );

                CREATE TABLE Employee
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    FirstName   NVARCHAR(100)   NOT NULL,
                    LastName    NVARCHAR(100)   NOT NULL,
                    Email       VARCHAR(255)    NOT NULL,
                    Phone       VARCHAR(30)     NULL,
                    HireDate    DATE            NOT NULL,
                    CompanyId   INT             NOT NULL,
                    AddressId   INT             NULL,
                    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    ModifiedAt  DATETIME2       NULL,

                    CONSTRAINT FK_Employee_Company
                      FOREIGN KEY (CompanyId)
                      REFERENCES Company(Id),
                    CONSTRAINT FK_Employee_Address
                      FOREIGN KEY (AddressId)
                      REFERENCES Address(Id)
                );

                CREATE TABLE Manager
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    EmployeeId  INT             NOT NULL,
                    Department  NVARCHAR(100)   NOT NULL,
                    Level       TINYINT         NOT NULL DEFAULT 1,
                    StartDate   DATE            NOT NULL,
                    EndDate     DATE            NULL,

                    CONSTRAINT FK_Manager_Employee
                      FOREIGN KEY (EmployeeId)
                      REFERENCES Employee(Id)
                );

                ALTER TABLE Employee
                ADD ManagerId INT NULL,
                    CONSTRAINT FK_Employee_Manager
                      FOREIGN KEY (ManagerId)
                      REFERENCES Manager(Id);

                CREATE TABLE Customer
                (
                    Id                INT IDENTITY(1,1) PRIMARY KEY,
                    FirstName         NVARCHAR(100)   NOT NULL,
                    LastName          NVARCHAR(100)   NOT NULL,
                    Email             VARCHAR(255)    NOT NULL,
                    Phone             VARCHAR(30)     NULL,
                    CompanyId         INT             NULL,
                    BillingAddressId  INT             NULL,
                    ShippingAddressId INT             NULL,
                    CreatedAt         DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    ModifiedAt        DATETIME2       NULL,

                    CONSTRAINT FK_Customer_Company
                       FOREIGN KEY (CompanyId)
                       REFERENCES Company(Id),
                    CONSTRAINT FK_Customer_BillingAddress
                       FOREIGN KEY (BillingAddressId)
                       REFERENCES Address(Id),
                    CONSTRAINT FK_Customer_ShippingAddress
                       FOREIGN KEY (ShippingAddressId)
                       REFERENCES Address(Id)
                );

                CREATE TABLE Product
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    Sku         VARCHAR(50)     NOT NULL,
                    Name        NVARCHAR(200)   NOT NULL,
                    Description NVARCHAR(MAX)   NULL,
                    UnitPrice   DECIMAL(18,2)   NOT NULL,
                    StockQty    INT             NOT NULL DEFAULT 0,
                    IsActive    BIT             NOT NULL DEFAULT 1,
                    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    ModifiedAt  DATETIME2       NULL,

                    CONSTRAINT UQ_Product_Sku UNIQUE (Sku)
                );

                CREATE TABLE [Order]
                (
                    Id                INT IDENTITY(1,1) PRIMARY KEY,
                    OrderNumber       VARCHAR(30)     NOT NULL,
                    CustomerId        INT             NOT NULL,
                    OrderDate         DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    Status            VARCHAR(20)     NOT NULL DEFAULT 'Pending',
                    SubTotal          DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    Tax               DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    Total             DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    Notes             NVARCHAR(1000)  NULL,
                    ShippingAddressId INT             NULL,
                    CreatedAt         DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
                    ModifiedAt        DATETIME2       NULL,

                    CONSTRAINT UQ_Order_OrderNumber
                      UNIQUE (OrderNumber),
                    CONSTRAINT FK_Order_Customer
                      FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
                    CONSTRAINT FK_Order_ShippingAddress
                      FOREIGN KEY (ShippingAddressId)
                      REFERENCES Address(Id)
                );

                CREATE TABLE OrderItem
                (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    OrderId     INT             NOT NULL,
                    ProductId   INT             NOT NULL,
                    Quantity    INT             NOT NULL,
                    UnitPrice   DECIMAL(18,2)   NOT NULL,
                    Discount    DECIMAL(18,2)   NOT NULL DEFAULT 0,
                    LineTotal   AS (Quantity * UnitPrice - Discount) PERSISTED,

                    CONSTRAINT FK_OrderItem_Order
                      FOREIGN KEY (OrderId)
                      REFERENCES [Order](Id)
                      ON DELETE CASCADE,
                    CONSTRAINT FK_OrderItem_Product
                      FOREIGN KEY (ProductId)
                      REFERENCES Product(Id),
                    CONSTRAINT CK_OrderItem_Quantity
                      CHECK (Quantity > 0)
                );

                CREATE INDEX IX_Employee_CompanyId ON Employee(CompanyId);
                CREATE INDEX IX_Employee_ManagerId ON Employee(ManagerId);
                CREATE INDEX IX_Customer_CompanyId ON Customer(CompanyId);
                CREATE INDEX IX_Customer_Email ON Customer(Email);
                CREATE INDEX IX_Order_CustomerId ON [Order](CustomerId);
                CREATE INDEX IX_Order_OrderDate ON [Order](OrderDate);
                CREATE INDEX IX_Order_Status ON [Order](Status);
                CREATE INDEX IX_OrderItem_OrderId ON OrderItem(OrderId);
                CREATE INDEX IX_OrderItem_ProductId ON OrderItem(ProductId);
                CREATE INDEX IX_Address_CompanyId ON Address(CompanyId);
                -- end-snippet
                """;
            await command.ExecuteNonQueryAsync();
        }

        var markdown = await SqlServerToMermaid.RenderMarkdown(database.Connection);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task CustomSchema()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText = "create schema sales;";
            await command.ExecuteNonQueryAsync();

            command.CommandText =
                """
                create table sales.Customers (
                    CustomerId int not null primary key,
                    Name nvarchar(50) not null
                );

                create table sales.Orders (
                    OrderId int not null primary key,
                    CustomerId int not null,
                    constraint FK_Orders_Customers
                      foreign key (CustomerId)
                      references sales.Customers(CustomerId)
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        var markdown = await SqlServerToMermaid.RenderMarkdown(database.Connection);

        await Verify(markdown, extension: "md");
    }
}
