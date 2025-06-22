namespace ProjektMaui.Views;

public partial class MainPage : ContentPage
{
    private string Name { get; set; }
    private string Role { get; set; }
    private string _token;
    private int _userId;


    public MainPage(string name, string role, string token, int userId)
    {
        InitializeComponent();

        Name = name;
        Role = role;
        _token = token;
        _userId = userId;

        System.Diagnostics.Debug.WriteLine($"MainPage: Token odebrany: {_token}");

        WelcomeLabel.Text = $"Witaj, {Name}!";

        LoadPanel();
    }

    private async void LoadPanel()
    {
        if (Role == "Admin")
        {
            AdminPanel.IsVisible = true;
            await AdminPanel.FadeTo(1, 800);
        }
        else
        {
            UserPanel.IsVisible = true;
            await UserPanel.FadeTo(1, 800);
        }
    }

    private async void OnManageUsers(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ManageUsersPage(_token));
    }

    private async void OnManageProducts(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ManageProductsPage(_token));
    }


    private async void OnViewReports(object sender, EventArgs e)
    {
        await DisplayAlert("Raporty", "Tutaj będą raporty.", "OK");
    }

    private async void OnAddProduct(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage(_token));
    }

    private async void OnViewOrders(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrdersListPage(_token));
    }

    private async void OnCatalogClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogPage());
    }

    private async void OnMyOrdersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MyOrdersPage(_token));
    }

    private async void OnViewMyData(object sender, EventArgs e)
    {
        // Przekaż token i userId!
        await Navigation.PushAsync(new MyProfilePage(_token, _userId));
    }


    private async void OnReportIssue(object sender, EventArgs e)
    {
        await DisplayAlert("Zgłoś", "Tutaj możesz zgłosić problem.", "OK");
    }
}
