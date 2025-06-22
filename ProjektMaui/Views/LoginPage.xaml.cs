using System.Text.Json;
using System.Text;
using Microsoft.Maui.Storage; // dla Preferences

namespace ProjektMaui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Błąd", "Wypełnij wszystkie pola", "OK");
            return;
        }

        var loginData = new { Email = email, Password = password };
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            var response = await client.PostAsync("/api/user/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null)
                {
                    // Zapisz token na wszelki wypadek
                    Preferences.Default.Set("jwt_token", result.Token);
                    System.Diagnostics.Debug.WriteLine($"LoginPage: Token zapisany: {result.Token}");

                    await DisplayAlert("Sukces", $"Witaj, {result.Name}!", "OK");

                    // Przejdź do MainPage — PRZEKAZUJ TOKEN!
                    await Navigation.PushAsync(new MainPage(result.Name, result.Role, result.Token, result.UserId));
                }
                else
                {
                    await DisplayAlert("Błąd", "Nie udało się odczytać danych logowania.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Błąd logowania", "Nieprawidłowy email lub hasło.", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Błąd", "Problem z połączeniem z serwerem.", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
