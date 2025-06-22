


using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class AddProductPage : ContentPage
{
    private string _token;
    private FileResult _selectedImage;

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

        string imageUrl = "";

        if (_selectedImage != null)
        {
            imageUrl = await UploadToBlobAsync(_selectedImage);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }
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
                await DisplayAlert("Sukces", "Produkt zosta³ dodany!", "OK");
                await Navigation.PopAsync(); // wróæ do strony admina
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê dodaæ produktu", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Problem z po³¹czeniem: {ex.Message}", "OK");
        }
    }

    private async void OnPickImageClicked(object sender, EventArgs e)
    {
        try
        {
            _selectedImage = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz zdjêcie produktu",
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
    private async Task<string> UploadToBlobAsync(FileResult file)
    {
        try
        {
            var stream = await file.OpenReadAsync();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string blobBaseUrl = "https://blobyprojekt.blob.core.windows.net/produkty";
            string sasToken = "sp=racwd&st=2025-06-22T19:37:38Z&se=2025-06-25T03:37:38Z&spr=https&sv=2024-11-04&sr=c&sig=IKCU%2FG8Ey9k%2BnkOJLSsEJf6y907POSiLg%2F17f4VxE7o%3D"; // bez `?` na pocz¹tku

            var fullUrl = $"{blobBaseUrl}/{fileName}?{sasToken}";


            using var httpClient = new HttpClient();
            using var content = new StreamContent(stream);
            content.Headers.Add("x-ms-blob-type", "BlockBlob");

            var response = await httpClient.PutAsync(fullUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return $"{blobBaseUrl}/{fileName}";
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê przes³aæ pliku do Azure Blob Storage", "OK");
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

