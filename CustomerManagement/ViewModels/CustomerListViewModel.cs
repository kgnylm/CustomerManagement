using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CustomerManagement.Models;
using CustomerManagement.Services;
using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CustomerManagement.ViewModels;

public class CustomerListViewModel : ViewModelBase
{
    private readonly ICustomerService _customerService;
    private readonly ICategoryService _categoryService;
    private readonly MainWindowViewModel _mainWindowViewModel;
    private ObservableCollection<Customer> _customers;
    private ObservableCollection<Category> _categories;
    private ObservableCollection<Category> _allCategories;
    private Customer? _selectedCustomer;
    private Category? _selectedCategory;
    private bool _isLoading;
    private string _searchText = string.Empty;

    public CustomerListViewModel(ICustomerService customerService, ICategoryService categoryService, MainWindowViewModel mainWindowViewModel)
    {
        _customerService = customerService;
        _categoryService = categoryService;
        _mainWindowViewModel = mainWindowViewModel;
        _customers = new ObservableCollection<Customer>();
        _categories = new ObservableCollection<Category>();
        _allCategories = new ObservableCollection<Category>();

        AddCustomerCommand = new RelayCommand(AddCustomer);
        EditCustomerCommand = new RelayCommand(EditCustomer, () => SelectedCustomer != null);
        DeleteCustomerCommand = new RelayCommand(DeleteCustomer, () => SelectedCustomer != null);
        RefreshCommand = new RelayCommand(LoadData);
    }

    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set => SetField(ref _customers, value);
    }

    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => SetField(ref _categories, value);
    }

    public ObservableCollection<Category> AllCategories
    {
        get => _allCategories;
        set => SetField(ref _allCategories, value);
    }

    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetField(ref _selectedCustomer, value))
            {
                (EditCustomerCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (DeleteCustomerCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetField(ref _selectedCategory, value))
            {
                _ = LoadCustomersByCategory();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetField(ref _searchText, value))
            {
                FilterCustomers();
            }
        }
    }

    public ICommand AddCustomerCommand { get; }
    public ICommand EditCustomerCommand { get; }
    public ICommand DeleteCustomerCommand { get; }
    public ICommand RefreshCommand { get; }

    public async Task Initialize()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            IsLoading = true;
            var customers = await _customerService.GetAllCustomersAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            Customers = new ObservableCollection<Customer>(customers);
            Categories = new ObservableCollection<Category>(categories);
            
            // Hepsi seçeneğini ekle
            var allCategory = new Category { CategoryId = 0, CategoryName = "Hepsi", CategoryDescription = "Tüm kategoriler" };
            var allCategories = new List<Category> { allCategory };
            allCategories.AddRange(categories);
            AllCategories = new ObservableCollection<Category>(allCategories);
            
            // Başlangıçta "Hepsi" seçili olsun
            if (SelectedCategory == null)
            {
                SelectedCategory = allCategory;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCustomersByCategory()
    {
        try
        {
            IsLoading = true;
            
            // Önce tüm müşterileri al
            var allCustomers = await _customerService.GetAllCustomersAsync();
            
            // Kategori filtresini uygula
            var customers = SelectedCategory == null || SelectedCategory.CategoryId == 0
                ? allCustomers
                : allCustomers.Where(c => c.CustomerCategory == SelectedCategory.CategoryId);
                
            // Arama filtresini uygula
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.CustomerCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            
            Customers = new ObservableCollection<Customer>(customers);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void FilterCustomers()
    {
        _ = LoadCustomersByCategory();
    }

    private Task AddCustomer()
    {
        _mainWindowViewModel.ShowCustomerEdit();
        return Task.CompletedTask;
    }

    private Task EditCustomer()
    {
        if (SelectedCustomer != null)
        {
            _mainWindowViewModel.ShowCustomerEdit(SelectedCustomer);
        }
        return Task.CompletedTask;
    }

    private async Task DeleteCustomer()
    {
        if (SelectedCustomer == null) return;

        try
        {
            IsLoading = true;
            await _customerService.DeleteCustomerAsync(SelectedCustomer.CustomerId);
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = null;
        }
        finally
        {
            IsLoading = false;
        }
    }
} 