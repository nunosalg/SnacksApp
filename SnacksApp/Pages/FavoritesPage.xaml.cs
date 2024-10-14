using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;

    public FavoritesPage(ApiService apiService, IValidator validator, FavoritesService favoritesService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoritesService = favoritesService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }

    private async Task GetFavoriteProducts()
    {
        try
        {
            var favoriteProducts = await _favoritesService.ReadAllAsync();

            if (favoriteProducts is null || favoriteProducts.Count == 0)
            {
                CvProducts.ItemsSource = null;//limpa a lista atual
                LblWarning.IsVisible = true; //mostra o aviso
            }
            else
            {
                CvProducts.ItemsSource = favoriteProducts;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "OK");
        }
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator, _favoritesService));

        ((CollectionView)sender).SelectedItem = null;
    }
}