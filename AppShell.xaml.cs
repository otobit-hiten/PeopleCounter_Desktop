using PeopleCounterDesktop.Views;
using PeopleCounterDesktop.ViewModels;

namespace PeopleCounterDesktop
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Routing.RegisterRoute(
               nameof(BuildingSensorsPage),
               typeof(BuildingSensorsPage));
        }
    }
}
