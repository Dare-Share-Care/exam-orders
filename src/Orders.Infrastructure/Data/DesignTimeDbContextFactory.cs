using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orders.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderContext>
{
    public OrderContext CreateDbContext(string[] args)
    {
        //TODO: Move connection string to appsettings.json
        var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
        optionsBuilder.UseSqlServer("Server=mssql-orders;Database=MTOGOOrders;User Id=sa;Password=thisIsSuperStrong1234;TrustServerCertificate=True");

        return new OrderContext(optionsBuilder.Options);
    }
}