using System;

namespace CustomerManagement.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int CustomerCategory { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdateDate { get; set; }
    
    // Navigation property
    public Category? Category { get; set; }
} 