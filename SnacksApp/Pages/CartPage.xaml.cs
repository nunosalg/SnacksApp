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

        bool savedAddress = Preferences.ContainsKey("address");

        if (savedAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phone = Preferences.Get("phone", string.Empty);

            LblAddress.Text = $"{name}\n{address}\n{phone}";
        }
        else
        {
            LblAddress.Text = "Type your address";
        }
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

    private async void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PurchaseCartItem cartItem)
        {
            cartItem.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "increase");
        }
    }

    private async void BtnDecrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PurchaseCartItem cartItem)
        {
            if (cartItem.Quantity == 1) return;
            else
            {
                cartItem.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "decrease");
            }
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is PurchaseCartItem cartItem)
        {
            bool response = await DisplayAlert("Confirm",
                          "Delete item from the purhase cart?", "Yes", "No");
            if (response)
            {
                PurchaseCartItems.Remove(cartItem);
                UpdateTotalPrice();
                await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "delete");
            }
        }
    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }
}