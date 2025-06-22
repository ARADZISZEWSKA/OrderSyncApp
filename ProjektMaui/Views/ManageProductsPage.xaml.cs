namespace ProjektMaui.Views;

using ProjektMaui.Models; 
using System.Net.Http.Headers;
using System.Text.Json;

public partial class ManageProductsPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public ManageProductsPage(string token)
    {
        InitializeComponent();
        _token = token;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net/api/")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);

        LoadProducts();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        try
        {
            var response = await _httpClient.GetAsync("products");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ProductsCollection.ItemsSource = products;
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ produktów", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }


    private async void OnEditProduct(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Product;
        if (product != null)
        {
            await Navigation.PushAsync(new EditProductPage(_token, product));
        }
    }


    private async void OnDeleteProduct(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Product;
        if (product != null)
        {
            var confirm = await DisplayAlert("Usuñ", $"Na pewno usun¹æ {product.Name}?", "Tak", "Nie");
            if (confirm)
            {
                var response = await _httpClient.DeleteAsync($"products/{product.Id}");
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Usuniêto", "Produkt usuniêty", "OK");
                    LoadProducts(); // odœwie¿ listê
                }
                else
                {
                    await DisplayAlert("B³¹d", "Nie uda³o siê usun¹æ produktu", "OK");
                }
            }
        }
    }
}
