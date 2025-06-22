using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.IO;


namespace ProjektMaui.Views;

public partial class OrdersListPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _jwtToken;
    private List<OrderDto> _allOrders = new();


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


        StatusPicker.ItemsSource = new List<string> { "All", "Received", "InProgress", "Completed", "Cancelled" };

        // 🟢 Potem dopiero ustaw domyślny index:
        StatusPicker.SelectedIndex = 0;

        // Ustaw daty:
        FromDatePicker.Date = DateTime.Now.AddMonths(-1);
        ToDatePicker.Date = DateTime.Now;


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
                _allOrders = JsonSerializer.Deserialize<List<OrderDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                OrdersCollectionView.ItemsSource = _allOrders;
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
    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (_allOrders == null || !_allOrders.Any())
        {
            DisplayAlert("Brak danych", "Nie ma zamówień do filtrowania.", "OK");
            return;
        }

        var fromDate = FromDatePicker.Date;
        var toDate = ToDatePicker.Date.AddDays(1).AddSeconds(-1); // Uwzględnij cały dzień do końca

        // Pobierz wybrany status
        var selectedStatus = StatusPicker.SelectedItem?.ToString() ?? "All";

        var filtered = _allOrders
            .Where(o =>
                o.CreatedAt >= fromDate &&
                o.CreatedAt <= toDate &&
                (selectedStatus == "All" || o.Status.ToString() == selectedStatus))
            .ToList();

        OrdersCollectionView.ItemsSource = filtered;
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

            // TYLKO TAK — gotowy obiekt DTO, MAŁA LITERA
            var dto = new { status = action };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/orders/{orderId}/status", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", $"Status zmieniony na: {action}", "OK");
                await LoadOrdersAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Błąd", $"Zmiana statusu nie powiodła się ({response.StatusCode}): {error}", "OK");
            }
        }
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        if (OrdersCollectionView.ItemsSource is not List<OrderDto> orders || !orders.Any())
        {
            await DisplayAlert("Błąd", "Brak danych do eksportu.", "OK");
            return;
        }

        try
        {
            // Buduj CSV
            var csv = new StringBuilder();
            csv.AppendLine("Id;Product;Notes;User;Status;CreatedAt");

            foreach (var o in orders)
            {
                csv.AppendLine($"{o.Id};{o.Product.Name};{o.Notes};{o.User.FirstName} {o.User.LastName};{o.Status};{o.CreatedAt:yyyy-MM-dd HH:mm}");
            }

            // Ścieżka do pliku w katalogu aplikacji
            var fileName = $"Zamowienia_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);

            await DisplayAlert("Sukces", $"Plik CSV zapisany:\n{filePath}", "OK");

            // Opcjonalnie: otwórz plik
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Nie udało się zapisać pliku.\n{ex.Message}", "OK");
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

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    



}

public enum OrderStatus
{
    Received,
    InProgress,
    Completed,
    Cancelled
}
