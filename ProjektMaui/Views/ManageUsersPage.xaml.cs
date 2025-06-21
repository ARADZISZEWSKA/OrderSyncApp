using ProjektMaui.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;


namespace ProjektMaui.Views;

public partial class ManageUsersPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _jwtToken;

    public ManageUsersPage(string jwtToken) // 🔑 TYLKO Z PARAMETREM
    {
        InitializeComponent();

        _jwtToken = jwtToken;
        System.Diagnostics.Debug.WriteLine($"ManageUsersPage: Token odebrany: {_jwtToken}");

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _jwtToken);

        LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/user/all");
            System.Diagnostics.Debug.WriteLine($"ManageUsersPage: StatusCode: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UserDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                UsersCollectionView.ItemsSource = users;
            }
            else
            {
                await DisplayAlert("Błąd", "Nie udało się pobrać użytkowników.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", ex.Message, "OK");
        }
    }

    private async void OnMakeAdminClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int userId)
        {
            var body = new { newRole = "Admin" };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/user/{userId}/role", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Rola zmieniona na Admina!", "OK");
                await LoadUsersAsync();
            }
            else
            {
                await DisplayAlert("Błąd", "Nie udało się zmienić roli.", "OK");
            }
        }
    }
}
