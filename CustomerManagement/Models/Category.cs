using System.Collections.Generic;

namespace CustomerManagement.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
} 