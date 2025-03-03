using Microsoft.EntityFrameworkCore;
using CustomerManagement.Models;

namespace CustomerManagement.Data;

public class CustomerManagementContext : DbContext
{
    public CustomerManagementContext(DbContextOptions<CustomerManagementContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data for Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Customer", CategoryDescription = "Customers" },
            new Category { CategoryId = 2, CategoryName = "Supplier", CategoryDescription = "Suppliers" },
            new Category { CategoryId = 3, CategoryName = "Distributor", CategoryDescription = "Distributors or Agent" },
            new Category { CategoryId = 4, CategoryName = "VIP", CategoryDescription = "VIP Customers" }
        );
    }
} 