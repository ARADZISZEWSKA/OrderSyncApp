using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirm = ConfirmPasswordEntry.Text;
        string firstName = FirstNameEntry.Text?.Trim();
        string lastName = LastNameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm)
            || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            await DisplayAlert("B³¹d", "Wype³nij wszystkie pola", "OK");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("B³¹d", "Has³a siê nie zgadzaj¹", "OK");
            return;
        }

        var registerData = new
        {
            Email = email,
            Password = password,
            ConfirmPassword = confirm,
            FirstName = firstName,
            LastName = lastName
        };

        var json = JsonSerializer.Serialize(registerData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net");

            var response = await client.PostAsync("/api/user/register", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Rejestracja zakoñczona pomyœlnie", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                await DisplayAlert("B³¹d rejestracji", errorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê po³¹czyæ z serwerem.\n{ex.Message}", "OK");
        }
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

}
