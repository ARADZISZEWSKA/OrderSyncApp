namespace ProjektMaui
{
    public partial class App : Application
    {
        // 
        public static string JwtToken => Preferences.Default.Get("jwt_token", string.Empty);

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
