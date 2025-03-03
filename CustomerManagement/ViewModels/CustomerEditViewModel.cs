using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using CustomerManagement.Models;
using CustomerManagement.Services;
using CustomerManagement.Views;
using Avalonia.Controls;
using System.Windows;

namespace CustomerManagement.ViewModels;

public class CustomerEditViewModel : ViewModelBase
{
    private readonly ICustomerService _customerService;
    private readonly ICategoryService _categoryService;
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly Customer _customer;
    private ObservableCollection<Category> _categories;
    private bool _isNew;
    private bool _isLoading;
    private string _customerCode = string.Empty;
    private string _customerName = string.Empty;
    private Category? _selectedCategory;
    private string _email = string.Empty;
    private string _phone = string.Empty;

    public CustomerEditViewModel(ICustomerService customerService, ICategoryService categoryService, MainWindowViewModel mainWindowViewModel, Customer? customer = null)
    {
        _customerService = customerService;
        _categoryService = categoryService;
        _mainWindowViewModel = mainWindowViewModel;
        _categories = new ObservableCollection<Category>();
        _isNew = customer == null;
        _customer = customer ?? new Customer();

        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
        AddCategoryCommand = new RelayCommand(AddCategory);

        if (!_isNew)
        {
            CustomerCode = _customer.CustomerCode;
            CustomerName = _customer.CustomerName;
            Email = _customer.Email;
            Phone = _customer.Phone;
        }
    }

    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => SetField(ref _categories, value);
    }

    public string CustomerCode
    {
        get => _customerCode;
        set
        {
            if (SetField(ref _customerCode, value))
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
                {
                    SetError(nameof(CustomerCode), "Müşteri kodu en az 2 karakter olmalıdır.");
                }
                else
                {
                    ClearErrors(nameof(CustomerCode));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string CustomerName
    {
        get => _customerName;
        set
        {
            if (SetField(ref _customerName, value))
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
                {
                    SetError(nameof(CustomerName), "Müşteri adı en az 3 karakter olmalıdır.");
                }
                else
                {
                    ClearErrors(nameof(CustomerName));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
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
                if (value == null)
                {
                    SetError(nameof(SelectedCategory), "Lütfen bir kategori seçin.");
                }
                else
                {
                    ClearErrors(nameof(SelectedCategory));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetField(ref _email, value))
            {
                if (!string.IsNullOrWhiteSpace(value) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    SetError(nameof(Email), "Geçerli bir e-posta adresi giriniz.");
                }
                else
                {
                    ClearErrors(nameof(Email));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if (SetField(ref _phone, value))
            {
                if (!string.IsNullOrWhiteSpace(value) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\+?[\d\s-]{10,}$"))
                {
                    SetError(nameof(Phone), "Geçerli bir telefon numarası giriniz.");
                }
                else
                {
                    ClearErrors(nameof(Phone));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddCategoryCommand { get; }

    public async Task Initialize()
    {
        try
        {
            IsLoading = true;
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);

            if (!_isNew)
            {
                var category = Categories.FirstOrDefault(c => c.CategoryId == _customer.CustomerCategory);
                if (category != null)
                {
                    SelectedCategory = category;
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task Save()
    {
        if (SelectedCategory == null) return;

        try
        {
            IsLoading = true;

            _customer.CustomerCode = CustomerCode;
            _customer.CustomerName = CustomerName;
            _customer.CustomerCategory = SelectedCategory.CategoryId;
            _customer.Email = Email;
            _customer.Phone = Phone;
            _customer.Category = SelectedCategory;

            if (_isNew)
            {
                await _customerService.AddCustomerAsync(_customer);
            }
            else
            {
                await _customerService.UpdateCustomerAsync(_customer);
            }

            _mainWindowViewModel.ShowCustomerList();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private Task Cancel()
    {
        _mainWindowViewModel.ShowCustomerList();
        return Task.CompletedTask;
    }

    private bool CanSave()
    {
        if (string.IsNullOrWhiteSpace(CustomerCode) || CustomerCode.Length < 2)
            return false;
            
        if (string.IsNullOrWhiteSpace(CustomerName) || CustomerName.Length < 3)
            return false;
            
        if (SelectedCategory == null)
            return false;
            
        if (!string.IsNullOrWhiteSpace(Email) && 
            !System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return false;
            
        if (!string.IsNullOrWhiteSpace(Phone) && 
            !System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^\+?[\d\s-]{10,}$"))
            return false;
            
        return !HasErrors;
    }

    private async Task AddCategory()
    {
        try
        {
            IsLoading = true;
            
            var resultSource = new TaskCompletionSource<Category?>();
            var viewModel = new CategoryEditViewModel(_categoryService, resultSource);
            var view = new CategoryEditView { DataContext = viewModel };
            
            var dialog = new Window
            {
                Title = "Yeni Kategori",
                Content = view,
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                CanResize = false,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            
            dialog.Show();
            
            var result = await resultSource.Task;
            if (result != null)
            {
                Categories.Add(result);
                SelectedCategory = result;
            }
            
            dialog.Close();
        }
        finally
        {
            IsLoading = false;
        }
    }
} 