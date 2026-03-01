class WithCommentsDbContext(DbContextOptions<WithCommentsDbContext> options) :
    DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Customer>(builder =>
            {
                builder.ToTable("Customers", t => t.HasComment("Core customer information"));
                builder.HasKey(_ => _.CustomerId);
                builder.Property(_ => _.CustomerId)
                    .HasColumnType("int")
                    .IsRequired()
                    .HasComment("Auto-generated identifier");
                builder.Property(_ => _.Name)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired()
                    .HasComment("Customer full name");
            });

        modelBuilder
            .Entity<Order>(builder =>
            {
                builder.ToTable("Orders", t => t.HasComment("Customer orders"));
                builder.HasKey(_ => _.OrderId);
                builder.Property(_ => _.OrderId)
                    .HasColumnType("int")
                    .IsRequired()
                    .HasComment("Auto-generated identifier");
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