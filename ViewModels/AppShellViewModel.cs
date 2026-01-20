using PeopleCounterDesktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace PeopleCounterDesktop.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        private readonly PeopleCounterSignalRService _signalR;
        private readonly PeopleCounterApiService _api;

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }


        public ICommand LogoutCommand { get; }
        public AppShellViewModel(
          PeopleCounterSignalRService signalR,
          PeopleCounterApiService api)
        {
            _signalR = signalR;
            _api = api;

            LogoutCommand = new Command(async () => await LogoutAsync());
            _ = LoadUserAsync();
        }

        private async Task LoadUserAsync()
        {
            UserName = await AuthCookieStore.GetUser() ?? "Guest";
        }

        private async Task LogoutAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Logout",
                "Cancel");

            if (!confirm)
                return;
            await _api.Logout();
            await _signalR.StopAsync();

            AuthCookieStore.ClearCookie();
            AuthCookieStore.ClearUser();

            await App.NavigateToShellAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
