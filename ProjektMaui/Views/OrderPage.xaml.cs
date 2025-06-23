using System.Text;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel;

namespace ProjektMaui.Views;

public partial class OrderPage : ContentPage
{
    private int _productId;
    private string _productName;
    private FileResult _selectedImage;
    private string _token;

    public OrderPage(int productId, string productName, string token)
    {
        InitializeComponent();
        _productId = productId;
        _productName = productName;
        _token = token;
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

    private async void OnSubmitOrder(object sender, EventArgs e)
    {
        string notes = NotesEditor.Text ?? "";
        string address = AddressEntry.Text ?? "";
        string imageUrl = "";

        if (_selectedImage != null)
        {
            imageUrl = await UploadToBlobAsync(_selectedImage);
            if (string.IsNullOrEmpty(imageUrl))
            {
                await DisplayAlert("B³¹d", "Upload zdjêcia siê nie powiód³.", "OK");
                return;
            }
        }

        var orderData = new
        {
            ProductId = _productId,
            Notes = notes,
            ImageUrl = imageUrl
        };

        var json = JsonSerializer.Serialize(orderData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            if (!string.IsNullOrEmpty(_token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }

            var response = await client.PostAsync("/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Zamówienie zosta³o wys³ane!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                await DisplayAlert("B³¹d", $"Nie uda³o siê wys³aæ zamówienia.\nStatus: {response.StatusCode}\n{errorBody}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Problem z po³¹czeniem: {ex.Message}", "OK");
        }
    }

    private async void OnUseLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                AddressEntry.Text = $"Lat: {location.Latitude}, Lon: {location.Longitude}";
            }
            else
            {
                await DisplayAlert("Lokalizacja", "Nie uda³o siê pobraæ lokalizacji", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Problem z lokalizacj¹: {ex.Message}", "OK");
        }
    }


    private async Task<string> UploadToBlobAsync(FileResult file)
    {
        try
        {
            var stream = await file.OpenReadAsync();
            if (stream.Length == 0)
            {
                await DisplayAlert("B³¹d", "Wybrany plik ma zerow¹ d³ugoœæ.", "OK");
                return null;
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string blobBaseUrl = "https://blobyprojekt.blob.core.windows.net/orders";
            string sasToken = "sp=racwdl&st=2025-06-22T23:09:43Z&se=2025-06-26T07:09:43Z&spr=https&sv=2024-11-04&sr=c&sig=57wjoLdrW5WoiamBK6esHp%2FQ2w%2BQQj%2BPysWvMC93nIo%3D";

            var fullUrl = $"{blobBaseUrl}/{fileName}?{sasToken}";

            using var httpClient = new HttpClient();
            using var content = new StreamContent(stream);
            content.Headers.Add("x-ms-blob-type", "BlockBlob");

            // Ustaw Content-Type na podstawie rozszerzenia pliku (tu uproszczone na jpg)
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            var response = await httpClient.PutAsync(fullUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return $"{blobBaseUrl}/{fileName}";
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                await DisplayAlert("B³¹d uploadu", $"Status: {response.StatusCode}\nTreœæ: {errorBody}", "OK");
                return null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"B³¹d uploadu: {ex.Message}", "OK");
            return null;
        }
    }
}
