using ProjektMaui.Models; // lub Twoja namespace z modelem Product
using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class EditProductPage : ContentPage
{
    private readonly string _token;
    private readonly Product _product;
    private FileResult _selectedImage;

    public EditProductPage(string token, Product product)
    {
        InitializeComponent();
        _token = token;
        _product = product;

        // Ustaw wstêpne wartoœci
        NameEntry.Text = _product.Name;
        DescriptionEditor.Text = _product.Description;
        PriceEntry.Text = _product.Price.ToString();

        if (!string.IsNullOrEmpty(_product.ImageUrl))
        {
            SelectedImage.Source = _product.ImageUrl;
            SelectedImage.IsVisible = true;
        }
    }

    private async void OnPickImageClicked(object sender, EventArgs e)
    {
        try
        {
            _selectedImage = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz nowe zdjêcie",
                FileTypes = FilePickerFileType.Images
            });

            if (_selectedImage != null)
            {
                var stream = await _selectedImage.OpenReadAsync();
                SelectedImage.Source = ImageSource.FromStream(() => stream);
                SelectedImage.IsVisible = true;

                await DisplayAlert("Info", $"Wybrano plik: {_selectedImage.FileName}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê wybraæ zdjêcia: {ex.Message}", "OK");
        }
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();
        string description = DescriptionEditor.Text?.Trim();
        string priceText = PriceEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(priceText))
        {
            await DisplayAlert("B³¹d", "Wype³nij wszystkie pola", "OK");
            return;
        }

        if (!decimal.TryParse(priceText, out decimal price))
        {
            await DisplayAlert("B³¹d", "Niepoprawna cena", "OK");
            return;
        }

        // Na razie: URL zostaje ten sam lub placeholder nowego pliku
        string imageUrl = _selectedImage?.FileName ?? _product.ImageUrl;

        var updatedProduct = new
        {
            Id = _product.Id,
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };

        var json = JsonSerializer.Serialize(updatedProduct);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.PutAsync($"/api/products/{_product.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Produkt zaktualizowany!", "OK");
                await Navigation.PopAsync(); // wróæ do ManageProductsPage
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê zaktualizowaæ produktu", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Problem z po³¹czeniem: {ex.Message}", "OK");
        }
    }
}
