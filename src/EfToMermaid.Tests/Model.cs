class SampleDbContext(DbContextOptions<SampleDbContext> options) :
    DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Customer>(builder =>
            {
                builder.ToTable("Customers");
                builder.HasKey(_ => _.CustomerId);
                builder.Property(_ => _.CustomerId)
                    .HasColumnType("int").IsRequired();
                builder.Property(_ => _.Name)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();
                builder.OwnsOne(_ => _.ShippingAddress, sa =>
                {
                    sa.Property(_ => _.Street)
                        .HasColumnType("nvarchar(100)")
                        .IsRequired();
                    sa.Property(_ => _.City)
                        .HasColumnType("nvarchar(50)")
                        .IsRequired();
                });
            });

        modelBuilder
            .Entity<Order>(builder =>
            {
                builder.ToTable("Orders");
                builder.HasKey(_ => _.OrderId);
                builder.Property(_ => _.OrderId)
                    .HasColumnType("int")
                    .IsRequired();
                builder.Property(_ => _.CustomerId)
                    .HasColumnType("int")
                    .IsRequired();

                builder.HasOne(_ => _.Customer)
                    .WithMany(_ => _.Orders)
                    .HasForeignKey(_ => _.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");
            });
    }
}

sealed class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = "";
    public StreetAddress ShippingAddress { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
}

sealed class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

sealed class NullableCustomer
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public List<NullableOrder> Orders { get; set; } = [];
}

sealed class NullableOrder
{
    public int OrderId { get; set; }
    public int? CustomerId { get; set; }
    public NullableCustomer? Customer { get; set; }
}

[Owned]
sealed class StreetAddress
{
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
}
