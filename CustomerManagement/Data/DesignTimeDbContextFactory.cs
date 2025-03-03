using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerManagement.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerManagementContext>
{
    public CustomerManagementContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerManagementContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CustomerManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");

        return new CustomerManagementContext(optionsBuilder.Options);
    }
} 