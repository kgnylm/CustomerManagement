using System.Threading.Tasks;
using System.Windows.Input;
using CustomerManagement.Models;
using CustomerManagement.Services;

namespace CustomerManagement.ViewModels;

public class CategoryEditViewModel : ViewModelBase
{
    private readonly ICategoryService _categoryService;
    private readonly TaskCompletionSource<Category?> _resultSource;
    private string _categoryName = string.Empty;
    private string _categoryDescription = string.Empty;
    private bool _isLoading;

    public CategoryEditViewModel(ICategoryService categoryService, TaskCompletionSource<Category?> resultSource)
    {
        _categoryService = categoryService;
        _resultSource = resultSource;

        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    public string CategoryName
    {
        get => _categoryName;
        set
        {
            if (SetField(ref _categoryName, value))
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
                {
                    SetError(nameof(CategoryName), "Kategori adı en az 2 karakter olmalıdır.");
                }
                else
                {
                    ClearErrors(nameof(CategoryName));
                }
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string CategoryDescription
    {
        get => _categoryDescription;
        set
        {
            if (SetField(ref _categoryDescription, value))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    SetError(nameof(CategoryDescription), "Kategori açıklaması boş olamaz.");
                }
                else
                {
                    ClearErrors(nameof(CategoryDescription));
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

    private async Task Save()
    {
        try
        {
            IsLoading = true;

            var category = new Category
            {
                CategoryName = CategoryName,
                CategoryDescription = CategoryDescription
            };

            var savedCategory = await _categoryService.AddCategoryAsync(category);
            _resultSource.SetResult(savedCategory);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private Task Cancel()
    {
        _resultSource.SetResult(null);
        return Task.CompletedTask;
    }

    private bool CanSave()
    {
        if (string.IsNullOrWhiteSpace(CategoryName) || CategoryName.Length < 2)
            return false;
            
        if (string.IsNullOrWhiteSpace(CategoryDescription))
            return false;
            
        return !HasErrors;
    }
} 