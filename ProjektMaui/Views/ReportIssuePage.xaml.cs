using System.Text;
using System.Text.Json;

namespace ProjektMaui.Views;

public partial class ReportIssuePage : ContentPage
{
    private readonly string _token;
    private readonly int _userId;
    private readonly string _userName;

    public ReportIssuePage(string token, int userId, string userName)
    {
        InitializeComponent();
        _token = token;
        _userId = userId;
        _userName = userName;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var content = ContentEditor.Text?.Trim();

        if (string.IsNullOrEmpty(content))
        {
            await DisplayAlert("B³¹d", "Podaj treœæ zg³oszenia!", "OK");
            return;
        }

        var report = new
        {
            UserId = _userId,
            UserName = _userName,
            Content = content
        };

        var json = JsonSerializer.Serialize(report);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://ordersyncbackend-akb4c5fmdhfkb9g3.polandcentral-01.azurewebsites.net/api/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.PostAsync("report", httpContent);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sukces", "Zg³oszenie zosta³o wys³ane!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("B³¹d", "Nie uda³o siê wys³aæ zg³oszenia", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
        }
    }
}
