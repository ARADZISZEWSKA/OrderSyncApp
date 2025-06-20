using ProjektMaui.Views;

namespace ProjektMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CatalogPage), typeof(CatalogPage));
        }

    }
}
