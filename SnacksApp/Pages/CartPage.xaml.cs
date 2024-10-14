using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.Collections.ObjectModel;

namespace SnacksApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private ObservableCollection<PurchaseCartItem> PurchaseCartItems = new ObservableCollection<PurchaseCartItem>();

    public CartPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetPurchaseCartItems();
    }

    private async Task<IEnumerable<PurchaseCartItem>> GetPurchaseCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (purchaseCartItems, errorMessage) = await
                     _apiService.GetPurchaseCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a p?gina de login
                await DisplayLoginPage();
                return Enumerable.Empty<PurchaseCartItem>();
            }

            if (purchaseCartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Couldn't obtain the items from the purchase cart.", "OK");
                return Enumerable.Empty<PurchaseCartItem>();
            }

            PurchaseCartItems.Clear();
            foreach (var item in purchaseCartItems)
            {
                PurchaseCartItems.Add(item);
            }

            CvCart.ItemsSource = PurchaseCartItems;
            UpdateTotalPrice();

            return purchaseCartItems;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "OK");
            return Enumerable.Empty<PurchaseCartItem>();
        }
    }

    private void UpdateTotalPrice()
    {
        try
        {
            var totalPrice = PurchaseCartItems.Sum(item => item.Price * item.Quantity);
            LblTotalPrice.Text = totalPrice.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error ocurred updating total price: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnDecrease_Clicked(object sender, EventArgs e)
    {

    }
}