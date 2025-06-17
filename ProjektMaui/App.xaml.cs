namespace ProjektMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
