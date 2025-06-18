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
        RoleLabel.Text = $"Jeste� zalogowany jako: {Role}";

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

    private async void OnViewMyData(object sender, EventArgs e)
    {
        await DisplayAlert("Dane", "Tutaj b�d� twoje dane.", "OK");
    }

    private async void OnReportIssue(object sender, EventArgs e)
    {
        await DisplayAlert("Zg�o�", "Tutaj mo�esz zg�osi� problem.", "OK");
    }
}
