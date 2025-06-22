using System.Text;
using System.Text.Json;
using ProjektMaui.Views;
using Microsoft.Maui.ApplicationModel;

namespace ProjektMaui.Views;

public partial class OrderPage : ContentPage
{
    private int _productId;
    private string _productName;
    private FileResult _selectedImage;

    public OrderPage(int productId, string productName)
    {
        InitializeComponent();

        _productId = productId;
        _productName = productName;

        ProductLabel.Text = _productName;
    }

    private async void OnPickImageClicked(object sender, EventArgs e)
    {
        try
        {
            _selectedImage = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz zdjêcie",
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

    private async void OnUseLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                AddressEntry.Text = $"LAT: {location.Latitude}, LNG: {location.Longitude}";
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ lokalizacji", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê pobraæ lokalizacji: {ex.Message}", "OK");
        }
    }


    private async void OnSubmitOrder(object sender, EventArgs e)
    {
        string notes = NotesEditor.Text ?? "";
        string address = AddressEntry.Text ?? "";
        string attachmentFileName = _selectedImage?.FileName ?? "";

        // Przygotowanie danych
        var orderData = new
        {
            ProductId = _productId,
            Notes = notes,
            Address = address,
            AttachmentFileName = attachmentFileName
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
