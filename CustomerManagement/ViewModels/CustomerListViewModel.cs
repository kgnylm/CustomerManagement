using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CustomerManagement.Models;
using CustomerManagement.Services;
using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

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
        ShowDetailsCommand = new RelayCommand(ShowCustomerDetails, () => SelectedCustomer != null);
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
                (ShowDetailsCommand as RelayCommand)?.NotifyCanExecuteChanged();
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
    public ICommand ShowDetailsCommand { get; }
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
            
            var allCategory = new Category { CategoryId = 0, CategoryName = "Hepsi", CategoryDescription = "Tüm kategoriler" };
            var allCategories = new List<Category> { allCategory };
            allCategories.AddRange(categories);
            AllCategories = new ObservableCollection<Category>(allCategories);
            
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
            
            var allCustomers = await _customerService.GetAllCustomersAsync();
            
            var customers = SelectedCategory == null || SelectedCategory.CategoryId == 0
                ? allCustomers
                : allCustomers.Where(c => c.CustomerCategory == SelectedCategory.CategoryId);
                
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

        var messageBox = new Window
        {
            Title = "Müşteri Silme",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            CanResize = false,
            ShowInTaskbar = false,
            SizeToContent = SizeToContent.Height
        };

        var panel = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 20
        };

        var messageText = new TextBlock
        {
            Text = $"{SelectedCustomer.CustomerName} isimli müşteriyi silmek istediğinize emin misiniz?",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Spacing = 10
        };

        var yesButton = new Button { Content = "Evet" };
        var noButton = new Button { Content = "Hayır" };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        panel.Children.Add(messageText);
        panel.Children.Add(buttonPanel);
        messageBox.Content = panel;

        var tcs = new TaskCompletionSource<bool>();

        yesButton.Click += async (s, e) =>
        {
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
            messageBox.Close();
            tcs.SetResult(true);
        };

        noButton.Click += (s, e) =>
        {
            messageBox.Close();
            tcs.SetResult(false);
        };

        messageBox.Show();
        await tcs.Task;
    }

    private Task ShowCustomerDetails()
    {
        if (SelectedCustomer == null) return Task.CompletedTask;

        var detailsWindow = new Window
        {
            Title = $"Müşteri Detayları - {SelectedCustomer.CustomerName}",
            Width = 400,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            CanResize = false,
            ShowInTaskbar = false,
            SizeToContent = SizeToContent.Height
        };

        var panel = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 10
        };

        var details = new[]
        {
            ("Müşteri ID:", SelectedCustomer.CustomerId.ToString()),
            ("Müşteri Kodu:", SelectedCustomer.CustomerCode),
            ("Müşteri Adı:", SelectedCustomer.CustomerName),
            ("Kategori:", SelectedCustomer.Category?.CategoryName ?? "Belirtilmemiş"),
            ("E-posta:", SelectedCustomer.Email),
            ("Telefon:", SelectedCustomer.Phone),
            ("Oluşturulma Tarihi:", SelectedCustomer.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")),
            ("Güncelleme Tarihi:", SelectedCustomer.UpdateDate.ToString("dd.MM.yyyy HH:mm:ss"))
        };

        foreach (var (label, value) in details)
        {
            var itemPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
            itemPanel.Children.Add(new TextBlock { Text = label, FontWeight = Avalonia.Media.FontWeight.Bold });
            itemPanel.Children.Add(new TextBlock { Text = value });
            panel.Children.Add(itemPanel);
        }

        var closeButton = new Button
        {
            Content = "Kapat",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 10, 0, 0)
        };

        closeButton.Click += (s, e) => detailsWindow.Close();
        panel.Children.Add(closeButton);
        
        detailsWindow.Content = panel;
        detailsWindow.Show();
        
        return Task.CompletedTask;
    }
} 