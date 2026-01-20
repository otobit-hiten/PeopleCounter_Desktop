using Microsoft.Extensions.DependencyInjection;
using PeopleCounterDesktop.Services;
using PeopleCounterDesktop.Views;

namespace PeopleCounterDesktop
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public App(PeopleCounterApiService apiService, IServiceProvider services)
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Services = services;
            InitializeComponent();
            MainPage = new ContentPage();
            _ = NavigateToShellAsync();
        }


        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                window.Title = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            }

            return window;
        }

        public static async Task NavigateToShellAsync()
        {
            var api = Services.GetRequiredService<PeopleCounterApiService>();

            var savedCookie = await AuthCookieStore.GetCookie();
            if (!string.IsNullOrEmpty(savedCookie))
            {
                ApiClient.CookieContainer.SetCookies(
                    ApiClient.Client.BaseAddress,
                    savedCookie
                );
            }

            var isLoggedIn = await api.IsLoggedInAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = isLoggedIn
                    ? Services.GetRequiredService<AppShell>()
                    : new LoginPage();
            });
        }

    }
}