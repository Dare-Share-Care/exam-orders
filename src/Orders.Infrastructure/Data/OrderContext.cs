using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Infrastructure.Entities;

namespace Orders.Infrastructure.Data;

public class OrderContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<RestaurantFee> RestaurantFees { get; set; }
    
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Primary keys
        modelBuilder.Entity<Order>().HasKey(order => order.Id);
        modelBuilder.Entity<OrderLine>().HasKey(orderLine => orderLine.Id);
        modelBuilder.Entity<RestaurantFee>().HasKey(restaurantFee => restaurantFee.Id);
        
        
        //Add Value Objects
        modelBuilder.Entity<Address>(ConfigureAddress);

        
        //Properties
        //Order TotalPrice
        modelBuilder.Entity<Order>().Property(order => order.TotalPrice)
            .HasColumnType("decimal(18,2)");
        
        //OrderLine Price
        modelBuilder.Entity<OrderLine>().Property(orderLine => orderLine.Price)
            .HasColumnType("decimal(18,2)");
        
        //RestaurantFee Amount
        modelBuilder.Entity<RestaurantFee>().Property(restaurantFee => restaurantFee.Amount)
            .HasColumnType("decimal(18,2)");
        
        //Relationships
        //Order to OrderLines
        modelBuilder.Entity<Order>()
            .HasMany(order => order.OrderLines)
            .WithOne(orderLine => orderLine.Order)
            .HasForeignKey(orderLine => orderLine.OrderId);
        
        //Order to RestaurantFee
        modelBuilder.Entity<Order>()
            .HasOne(order => order.RestaurantFee)
            .WithOne(restaurantFee => restaurantFee.Order)
            .HasForeignKey<RestaurantFee>(restaurantFee => restaurantFee.OrderId);
    }
    
    //Address value object
    void ConfigureAddress<T>(EntityTypeBuilder<T> entity) where T : Address
    {
        entity.ToTable("Address", "dbo");

        entity.Property<int>("Id")
            .IsRequired();
        entity.HasKey("Id");
    }
}