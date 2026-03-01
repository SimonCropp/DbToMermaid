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

    static async Task ScriptUsage()
    {
        #region SqlServerScriptUsage

        var script = """
            create table Customers (
                Id int primary key,
                Name nvarchar(100) not null
            );
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

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

                create table Company
                (
                    Id          int identity(1,1) primary key,
                    Name        nvarchar(200)   not null,
                    TaxNumber   varchar(50)     null,
                    Phone       varchar(30)     null,
                    Email       varchar(255)    null,
                    CreatedAt   datetime2       not null default getutcdate(),
                    ModifiedAt  datetime2       null
                );

                create table Employee
                (
                    Id          int identity(1,1) primary key,
                    FirstName   nvarchar(100)   not null,
                    LastName    nvarchar(100)   not null,
                    Email       varchar(255)    not null,
                    Phone       varchar(30)     null,
                    HireDate    date            not null,
                    CompanyId   int             not null,
                    CreatedAt   datetime2       not null default getutcdate(),
                    ModifiedAt  datetime2       null,

                    constraint FK_Employee_Company
                      foreign key (CompanyId)
                      references Company(Id),
                );

                create table Manager
                (
                    Id          int identity(1,1) primary key,
                    EmployeeId  int             not null,
                    Department  nvarchar(100)   not null,
                    Level       tinyint         not null default 1,
                    StartDate   date            not null,
                    EndDate     date            null,

                    constraint FK_Manager_Employee
                      foreign key (EmployeeId)
                      references Employee(Id)
                );
                -- rest of schema omitted from docs
                -- end-snippet
                alter table Employee
                add ManagerId int null,
                    constraint FK_Employee_Manager
                      foreign key (ManagerId)
                      references Manager(Id);

                create table Customer
                (
                    Id                int identity(1,1) primary key,
                    FirstName         nvarchar(100)   not null,
                    LastName          nvarchar(100)   not null,
                    Email             varchar(255)    not null,
                    Phone             varchar(30)     null,
                    CompanyId         int             null,
                    CreatedAt         datetime2       not null default getutcdate(),
                    ModifiedAt        datetime2       null,

                    constraint FK_Customer_Company
                       foreign key (CompanyId)
                       references Company(Id),
                );

                create table Product
                (
                    Id          int identity(1,1) primary key,
                    Sku         varchar(50)     not null,
                    Name        nvarchar(200)   not null,
                    Description nvarchar(max)   null,
                    UnitPrice   decimal(18,2)   not null,
                    StockQty    int             not null default 0,
                    IsActive    bit             not null default 1,
                    CreatedAt   datetime2       not null default getutcdate(),
                    ModifiedAt  datetime2       null,

                    constraint UQ_Product_Sku unique (Sku)
                );

                create table [Order]
                (
                    Id                int identity(1,1) primary key,
                    OrderNumber       varchar(30)     not null,
                    CustomerId        int             not null,
                    OrderDate         datetime2       not null default getutcdate(),
                    Status            varchar(20)     not null default 'Pending',
                    SubTotal          decimal(18,2)   not null default 0,
                    Tax               decimal(18,2)   not null default 0,
                    Total             decimal(18,2)   not null default 0,
                    Notes             nvarchar(1000)  null,
                    CreatedAt         datetime2       not null default getutcdate(),
                    ModifiedAt        datetime2       null,

                    constraint UQ_Order_OrderNumber
                      unique (OrderNumber),
                    constraint FK_Order_Customer
                      foreign key (CustomerId) references Customer(Id),
                );

                create table OrderItem
                (
                    Id          int identity(1,1) primary key,
                    OrderId     int             not null,
                    ProductId   int             not null,
                    Quantity    int             not null,
                    UnitPrice   decimal(18,2)   not null,
                    Discount    decimal(18,2)   not null default 0,
                    LineTotal   as (Quantity * UnitPrice - Discount) persisted,

                    constraint FK_OrderItem_Order
                      foreign key (OrderId)
                      references [Order](Id)
                      on delete cascade,
                    constraint FK_OrderItem_Product
                      foreign key (ProductId)
                      references Product(Id),
                    constraint CK_OrderItem_Quantity
                      check (Quantity > 0)
                );

                create index IX_Employee_CompanyId on Employee(CompanyId);
                create index IX_Employee_ManagerId on Employee(ManagerId);
                create index IX_Customer_CompanyId on Customer(CompanyId);
                create index IX_Customer_Email on Customer(Email);
                create index IX_Order_CustomerId on [Order](CustomerId);
                create index IX_Order_OrderDate on [Order](OrderDate);
                create index IX_Order_Status on [Order](Status);
                create index IX_OrderItem_OrderId on OrderItem(OrderId);
                create index IX_OrderItem_ProductId on OrderItem(ProductId);
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

    [Test]
    public async Task RenderMarkdownWithComments()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText =
                """
                create table Customers
                (
                    CustomerId  int identity(1,1) primary key,
                    Name        nvarchar(100)   not null,
                    Email       varchar(255)    null
                );

                exec sp_addextendedproperty
                    @name = N'MS_Description',
                    @value = N'Core customer information',
                    @level0type = N'schema', @level0name = N'dbo',
                    @level1type = N'table',  @level1name = N'Customers';

                exec sp_addextendedproperty
                    @name = N'MS_Description',
                    @value = N'Auto-generated identifier',
                    @level0type = N'schema', @level0name = N'dbo',
                    @level1type = N'table',  @level1name = N'Customers',
                    @level2type = N'column', @level2name = N'CustomerId';

                exec sp_addextendedproperty
                    @name = N'MS_Description',
                    @value = N'Customer full name',
                    @level0type = N'schema', @level0name = N'dbo',
                    @level1type = N'table',  @level1name = N'Customers',
                    @level2type = N'column', @level2name = N'Name';
                """;
            await command.ExecuteNonQueryAsync();
        }

        var markdown = await SqlServerToMermaid.RenderMarkdown(database.Connection);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithComments()
    {
        var script = """
            create table Customers
            (
                CustomerId  int primary key,
                Name        nvarchar(100)   not null,
                Email       varchar(255)    null
            );

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Core customer information',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Customers';

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Auto-generated identifier',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Customers',
                @level2type = N'column', @level2name = N'CustomerId';

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Customer full name',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Customers',
                @level2type = N'column', @level2name = N'Name';
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownWithEscaping()
    {
        await using var database = await instance.Build();
        await using (var command = database.Connection.CreateCommand())
        {
            command.CommandText =
                """
                create table Customers
                (
                    CustomerId  int identity(1,1) primary key,
                    Name        nvarchar(100)   not null
                );

                exec sp_addextendedproperty
                    @name = N'MS_Description',
                    @value = N'Contains "quotes" here',
                    @level0type = N'schema', @level0name = N'dbo',
                    @level1type = N'table',  @level1name = N'Customers';

                exec sp_addextendedproperty
                    @name = N'MS_Description',
                    @value = N'The "primary" key',
                    @level0type = N'schema', @level0name = N'dbo',
                    @level1type = N'table',  @level1name = N'Customers',
                    @level2type = N'column', @level2name = N'CustomerId';
                """;
            await command.ExecuteNonQueryAsync();
        }

        var markdown = await SqlServerToMermaid.RenderMarkdown(database.Connection);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task RenderMarkdownFromScript()
    {
        var script = """
            create table Company
            (
                Id          int primary key,
                Name        nvarchar(200)   not null,
                CreatedAt   datetime2       not null
            );

            create table Employee
            (
                Id          int primary key,
                FirstName   nvarchar(100)   not null,
                LastName    nvarchar(100)   not null,
                CompanyId   int             not null,
                Salary      decimal(18,2)   not null,
                Bonus       decimal(18,2)   not null,
                TotalPay    as (Salary + Bonus),

                constraint FK_Employee_Company
                  foreign key (CompanyId)
                  references Company(Id)
            );
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithEscaping()
    {
        var script = """
            create table Customers
            (
                CustomerId  int primary key,
                Name        nvarchar(100)   not null
            );

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Contains "quotes" here',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Customers';

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'The "primary" key',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Customers',
                @level2type = N'column', @level2name = N'CustomerId';
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithSchema()
    {
        var script = """
            create table sales.Customers
            (
                CustomerId int primary key,
                Name nvarchar(50) not null
            );

            create table sales.Orders
            (
                OrderId int primary key,
                CustomerId int not null,

                constraint FK_Orders_Customers
                  foreign key (CustomerId)
                  references sales.Customers(CustomerId)
            );
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithAlterTable()
    {
        var script = """
            create table Parent
            (
                Id int primary key
            );

            create table Child
            (
                Id int primary key,
                ParentId int null
            );

            alter table Child
            add constraint FK_Child_Parent
              foreign key (ParentId)
              references Parent(Id);
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithTableLevelPrimaryKey()
    {
        var script = """
            create type EmailAddress from varchar(255);

            create table Parent
            (
                Id int,
                Email EmailAddress null,
                primary key(Id)
            );

            create table Child
            (
                Id int primary key,
                ParentId int not null,
                foreign key (ParentId) references Parent(Id)
            );
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptWithAlterTableAddColumn()
    {
        var script = """
            create table Items
            (
                Id int primary key
            );

            alter table Items
            add Name nvarchar(100) not null;

            alter table NewTable
            add Value int null;
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }

    [Test]
    public async Task RenderMarkdownFromScriptIgnoresIrrelevantExec()
    {
        var script = """
            create table hr.Employees
            (
                Id int primary key,
                Name nvarchar(100) not null
            );

            exec('select 1');

            exec sp_rename @objname = N'old', @newname = N'new';

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Employee records',
                @level0type = N'schema', @level0name = N'hr',
                @level1type = N'table',  @level1name = N'Employees';

            exec sp_addextendedproperty
                @name = N'SomeOtherProperty',
                @value = N'ignored',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'Employees';

            exec sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'ghost',
                @level0type = N'schema', @level0name = N'dbo',
                @level1type = N'table',  @level1name = N'NonExistent';
            """;

        var markdown = await SqlServerToMermaid.RenderMarkdownFromScript(script);

        await Verify(markdown, extension: "md");
    }
}
