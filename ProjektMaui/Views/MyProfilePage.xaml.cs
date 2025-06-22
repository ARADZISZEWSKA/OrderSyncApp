using ProjektMaui.Models;
using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class MyProfilePage : ContentPage
{
    private readonly string _token;
    private readonly int _userId;
    private readonly HttpClient _httpClient;

    public MyProfilePage(string token, int userId)
    {
        InitializeComponent();
        _token = token;
        _userId = userId;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net/api/")
        };
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        LoadUser();
    }

    private async void LoadUser()
    {
        try
        {
            var response = await _httpClient.GetAsync($"user/{_userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                FirstNameEntry.Text = user.FirstName;
                LastNameEntry.Text = user.LastName;
                EmailEntry.Text = user.Email;
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê pobraæ danych", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var updatedUser = new
        {
            Id = _userId,
            FirstName = FirstNameEntry.Text?.Trim(),
            LastName = LastNameEntry.Text?.Trim(),
            Email = EmailEntry.Text?.Trim()
        };

        var json = JsonSerializer.Serialize(updatedUser);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PutAsync($"user/{_userId}", content);

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                await DisplayAlert("Sukces", "Dane zaktualizowane!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("B³¹d", $"Nie uda³o siê zapisaæ zmian. Status: {response.StatusCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }

}
