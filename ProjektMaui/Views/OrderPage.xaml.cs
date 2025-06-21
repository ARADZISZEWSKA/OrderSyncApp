using System.Text;
using System.Text.Json;
using ProjektMaui.Views;

namespace ProjektMaui.Views;

public partial class OrderPage : ContentPage
{
    private int _productId;

    public OrderPage(int productId)
    {
        InitializeComponent();

        _productId = productId;

        ProductLabel.Text = $"Produkt ID: {_productId}";
    }

    private async void OnSubmitOrder(object sender, EventArgs e)
    {
        string notes = NotesEditor.Text ?? "";

        // Przygotowanie danych
        var orderData = new
        {
            ProductId = _productId,
            Notes = notes
        };

        var json = JsonSerializer.Serialize(orderData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            // Dodaj token JWT
            var token = Preferences.Default.Get("jwt_token", string.Empty);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync("/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Zamówienie zosta³o wys³ane!", "OK");
                await Navigation.PopAsync(); // wróæ do CatalogPage
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê wys³aæ zamówienia", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Problem z po³¹czeniem: {ex.Message}", "OK");
        }
    }
}
