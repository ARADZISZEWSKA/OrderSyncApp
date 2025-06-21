using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class OrdersListPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _jwtToken;

    public OrdersListPage(string jwtToken)
    {
        InitializeComponent();

        _jwtToken = jwtToken;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _jwtToken);

        LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/orders/admin");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                OrdersCollectionView.ItemsSource = orders;
            }
            else
            {
                await DisplayAlert("Błąd", "Nie udało się pobrać zamówień.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", ex.Message, "OK");
        }
    }

    private async void OnChangeStatusClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            var param = button.CommandParameter?.ToString();
            if (!int.TryParse(param, out int orderId))
            {
                await DisplayAlert("Błąd", "Nie można odczytać ID zamówienia!", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Zmiana statusu dla OrderID: {orderId}");

            string action = await DisplayActionSheet(
                "Zmień status na:",
                "Anuluj",
                null,
                "InProgress", "Completed", "Cancelled");

            if (action == "Anuluj" || string.IsNullOrWhiteSpace(action))
                return;

            var content = new StringContent($"\"{action}\"",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"api/orders/{orderId}/status", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", $"Status zmieniony na: {action}", "OK");
                await LoadOrdersAsync();
            }
            else
            {
                await DisplayAlert("Błąd", $"Zmiana statusu nie powiodła się ({response.StatusCode})", "OK");
            }
        }
    }



}

public class OrderDto
{
    public int Id { get; set; }
    public ProductDto Product { get; set; } = new();
    public UserDto User { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }  

    public DateTime CreatedAt { get; set; }
}

public enum OrderStatus
{
    Received,
    InProgress,
    Completed,
    Cancelled
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
}
