using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblUserName.Text = "Hello, " + Preferences.Get("username", string.Empty);
        _apiService = apiService;
        _validator = validator;
        Title = AppConfig.HomePageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoriesList();
        await GetBestSellers();
        await GetPopulars();
    }

    private async Task<IEnumerable<Category>> GetCategoriesList()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve categories.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetBestSellers()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("bestseller", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve categories.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvBestSellers.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Product>> GetPopulars()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve categories.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvPopulars.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection == null) return;


        Navigation.PushAsync(new ProductsListPage(_apiService,
                                                  _validator,
                                                  currentSelection.Name!,
                                                  currentSelection.Id
                                                  ));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvBestSellers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CvPopulars_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}