using CustomerManagement.Services;
using CustomerManagement.Models;

namespace CustomerManagement.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ICustomerService _customerService;
    private readonly ICategoryService _categoryService;
    private ViewModelBase? _currentViewModel;

    public MainWindowViewModel(ICustomerService customerService, ICategoryService categoryService)
    {
        _customerService = customerService;
        _categoryService = categoryService;
        
        ShowCustomerList();
    }

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetField(ref _currentViewModel, value);
    }

    public void ShowCustomerList()
    {
        var customerListViewModel = new CustomerListViewModel(_customerService, _categoryService, this);
        _ = customerListViewModel.Initialize();
        CurrentViewModel = customerListViewModel;
    }

    public void ShowCustomerEdit(Customer? customer = null)
    {
        var customerEditViewModel = new CustomerEditViewModel(_customerService, _categoryService, this, customer);
        _ = customerEditViewModel.Initialize();
        CurrentViewModel = customerEditViewModel;
    }
}
