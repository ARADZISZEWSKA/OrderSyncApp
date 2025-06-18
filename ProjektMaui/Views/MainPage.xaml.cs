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

        // Ustawienie treœci labeli rêcznie
        WelcomeLabel.Text = $"Witaj, {Name}!";
        RoleLabel.Text = $"Jesteœ zalogowany jako: {Role}";

        // Pokazanie odpowiedniego panelu
        if (Role == "Admin")
            AdminPanel.IsVisible = true;
        else
            UserPanel.IsVisible = true;
    }

    private async void OnManageUsers(object sender, EventArgs e)
    {
        await DisplayAlert("Admin", "Tutaj bêdzie zarz¹dzanie u¿ytkownikami.", "OK");
    }

    private async void OnViewReports(object sender, EventArgs e)
    {
        await DisplayAlert("Raporty", "Tutaj bêd¹ raporty.", "OK");
    }

    private async void OnViewMyData(object sender, EventArgs e)
    {
        await DisplayAlert("Dane", "Tutaj bêd¹ twoje dane.", "OK");
    }

    private async void OnReportIssue(object sender, EventArgs e)
    {
        await DisplayAlert("Zg³oœ", "Tutaj mo¿esz zg³osiæ problem.", "OK");
    }
}
