using System.Text.Json;

namespace ProjektMaui.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage()
    {
        InitializeComponent();
        LoadProducts();
    }

    private async void LoadProducts()
    {
        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            var response = await client.GetAsync("/api/products");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var products = JsonSerializer.Deserialize<List<ProductDto>>(json, options);


                // Ustawiamy dane do CollectionView
                ProductsCollection.ItemsSource = products;
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ produktów", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"B³¹d po³¹czenia: {ex.Message}", "OK");
        }
    }

    private async void OnOrderClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = button?.BindingContext as ProductDto;

        if (product != null)
        {
            await Navigation.PushAsync(new OrderPage(product.Id, product.Name));
        }

    }


    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
