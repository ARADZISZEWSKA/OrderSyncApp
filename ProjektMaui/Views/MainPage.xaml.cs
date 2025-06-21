namespace ProjektMaui.Views;

public partial class MainPage : ContentPage
{
    private string Name { get; set; }
    private string Role { get; set; }
    private string _token; // 🔑 MUSI BYĆ

    public MainPage(string name, string role, string token) // 🔑 3 parametry
    {
        InitializeComponent();

        Name = name;
        Role = role;
        _token = token;

        System.Diagnostics.Debug.WriteLine($"MainPage: Token odebrany: {_token}");

        WelcomeLabel.Text = $"Witaj, {Name}!";
        RoleLabel.Text = $"Jesteś zalogowany jako: {Role}";

        if (Role == "Admin")
            AdminPanel.IsVisible = true;
        else
            UserPanel.IsVisible = true;
    }

    private async void OnManageUsers(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"MainPage: Token przekazywany do ManageUsersPage: {_token}");
        await Navigation.PushAsync(new ManageUsersPage(_token)); // 🔑 Token przekazany parametrem
    }

    // Reszta jak u Ciebie
    private async void OnViewReports(object sender, EventArgs e)
    {
        await DisplayAlert("Raporty", "Tutaj będą raporty.", "OK");
    }

    private async void OnAddProduct(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage(_token));
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
