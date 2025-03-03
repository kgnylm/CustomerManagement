using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerManagement.Models;

namespace CustomerManagement.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<Customer> AddCustomerAsync(Customer customer);
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(int id);
    Task<List<Customer>> GetCustomersByCategoryAsync(int categoryId);
} 