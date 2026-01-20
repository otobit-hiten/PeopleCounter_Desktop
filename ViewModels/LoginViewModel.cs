using Microsoft.Maui.Controls;
using PeopleCounterDesktop.Models;
using PeopleCounterDesktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PeopleCounterDesktop.ViewModels
{
    class LoginViewModel : BindableObject
    {
        private readonly PeopleCounterApiService _apiService = new();
        private string _username;
        public string Username
        {
            get => _username;
            set {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PasswordIcon));
            }
        }
        public string PasswordIcon => IsPasswordVisible ? "eye_off.png" : "eye.png";
        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
            TogglePasswordCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Username and password required", "OK");
                return;
            }

            try
            {
                IsLoading = true;
                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var result = await _apiService.Login(request);

                if (result)
                {
                    await AuthCookieStore.SaveUser(Username);
                    await App.NavigateToShellAsync();

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Login Failed", "Invalid credentials", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
