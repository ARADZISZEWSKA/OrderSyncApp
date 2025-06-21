using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class MyOrdersPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _jwtToken;

    public MyOrdersPage(string jwtToken)
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
            var response = await _httpClient.GetAsync("api/orders");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                OrdersCollectionView.ItemsSource = orders;
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ zamówieñ.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }
}

