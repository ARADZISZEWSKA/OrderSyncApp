using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class AddProductPage : ContentPage
{
    private string _token;

    public AddProductPage(string token)
    {
        InitializeComponent();

        _token = token;
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();
        string description = DescriptionEditor.Text?.Trim();
        string priceText = PriceEntry.Text?.Trim();
        string imageUrl = ImageUrlEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(priceText))
        {
            await DisplayAlert("B��d", "Wype�nij wszystkie pola", "OK");
            return;
        }

        if (!decimal.TryParse(priceText, out decimal price))
        {
            await DisplayAlert("B��d", "Niepoprawna cena", "OK");
            return;
        }

        var productData = new
        {
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };

        var json = JsonSerializer.Serialize(productData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            // Dodaj token JWT
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.PostAsync("/api/products", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Produkt zosta� dodany!", "OK");
                await Navigation.PopAsync(); // wr�� do strony admina
            }
            else
            {
                await DisplayAlert("B��d", "Nie uda�o si� doda� produktu", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B��d", $"Problem z po��czeniem: {ex.Message}", "OK");
        }
    }
}
