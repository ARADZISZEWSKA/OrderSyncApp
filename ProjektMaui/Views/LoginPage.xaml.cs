using System.Text.Json;
using System.Text;

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
            await DisplayAlert("B³¹d", "Wype³nij wszystkie pola", "OK");
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
                var result = JsonSerializer.Deserialize<LoginResponse>(responseBody);

                if (result != null)
                {
                    await DisplayAlert("Sukces", $"Witaj, {result.Name}!", "OK");

                    // Przejœcie do MainPage z przekazaniem Name i Role
                    await Navigation.PushAsync(new MainPage(result.Name, result.Role));

                    // ALBO jeœli u¿ywasz Shell, mo¿esz ustawiæ MainPage rêcznie:
                    // Application.Current.MainPage = new NavigationPage(new MainPage(result.Name, result.Role));
                }
                else
                {
                    await DisplayAlert("B³¹d", "Nie uda³o siê odczytaæ danych logowania", "OK");
                }
            }
            else
            {
                await DisplayAlert("B³¹d logowania", "Nieprawid³owy email lub has³o", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("B³¹d", "Problem z po³¹czeniem do serwera", "OK");
        }
    }

        private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;


    }
}

