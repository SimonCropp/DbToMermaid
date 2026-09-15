class WithTptDbContext(DbContextOptions<WithTptDbContext> options) :
    DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Vehicle>()
            .ToTable("Vehicles")
            .UseTptMappingStrategy();

        modelBuilder
            .Entity<Car>()
            .ToTable("Cars");
    }
}
