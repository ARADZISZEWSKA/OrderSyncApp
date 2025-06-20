namespace ProjektMaui.Views;

public partial class MainPage : ContentPage
{
    private string Name { get; set; }
    private string Role { get; set; }

    public MainPage(string name, string role)
    {
        InitializeComponent();

        Name = name;
        Role = role;

        // Ustawienie tre�ci labeli r�cznie
        WelcomeLabel.Text = $"Witaj, {Name}!";
        RoleLabel.Text = $"Jesteś zalogowany jako: {Role}";

        // Pokazanie odpowiedniego panelu
        if (Role == "Admin")
            AdminPanel.IsVisible = true;
        else
            UserPanel.IsVisible = true;
    }

    private async void OnManageUsers(object sender, EventArgs e)
    {
        await DisplayAlert("Admin", "Tutaj b�dzie zarz�dzanie u�ytkownikami.", "OK");
    }

    private async void OnViewReports(object sender, EventArgs e)
    {
        await DisplayAlert("Raporty", "Tutaj b�d� raporty.", "OK");
    }

    private async void OnCatalogClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogPage());
    }

    private async void OnViewMyData(object sender, EventArgs e)
    {
        await DisplayAlert("Dane", "Tutaj będą twoje dane.", "OK");
    }

    private async void OnReportIssue(object sender, EventArgs e)
    {
        await DisplayAlert("Zgłoś", "Tutaj możesz zgłosić problem.", "OK");
    }
}