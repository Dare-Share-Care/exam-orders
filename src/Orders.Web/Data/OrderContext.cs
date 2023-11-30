using Microsoft.EntityFrameworkCore;
using Orders.Web.Entities;

namespace Orders.Web.Data;

public class OrderContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Primary keys
        modelBuilder.Entity<Order>().HasKey(order => order.Id);
        modelBuilder.Entity<OrderLine>().HasKey(orderLine => orderLine.Id);
        
        //Properties
        //Order TotalPrice
        modelBuilder.Entity<Order>().Property(order => order.TotalPrice)
            .HasColumnType("decimal(18,2)");
        
        //OrderLine Price
        modelBuilder.Entity<OrderLine>().Property(orderLine => orderLine.Price)
            .HasColumnType("decimal(18,2)");
        
        //Relationships
        //Order to OrderLines
        modelBuilder.Entity<Order>()
            .HasMany(order => order.OrderLines)
            .WithOne(orderLine => orderLine.Order)
            .HasForeignKey(orderLine => orderLine.OrderId);
    }
}