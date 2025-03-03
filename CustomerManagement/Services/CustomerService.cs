using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerManagementContext _context;

    public CustomerService(CustomerManagementContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.Category)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        customer.CreatedDate = DateTime.Now;
        customer.UpdateDate = DateTime.Now;
        
        var category = await _context.Categories.FindAsync(customer.CustomerCategory);
        if (category != null)
        {
            customer.Category = category;
        }
        
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        var existingCustomer = await _context.Customers
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
            
        if (existingCustomer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customer.CustomerId} not found.");
        }

        existingCustomer.CustomerCode = customer.CustomerCode;
        existingCustomer.CustomerName = customer.CustomerName;
        existingCustomer.CustomerCategory = customer.CustomerCategory;
        existingCustomer.Email = customer.Email;
        existingCustomer.Phone = customer.Phone;
        existingCustomer.UpdateDate = DateTime.Now;

        var category = await _context.Categories.FindAsync(customer.CustomerCategory);
        if (category != null)
        {
            existingCustomer.Category = category;
        }

        await _context.SaveChangesAsync();
        return existingCustomer;
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Customer>> GetCustomersByCategoryAsync(int categoryId)
    {
        return await _context.Customers
            .Include(c => c.Category)
            .Where(c => c.CustomerCategory == categoryId)
            .ToListAsync();
    }
} 