using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using PeopleCounterDesktop.Models;
using PeopleCounterDesktop.Services;
using PeopleCounterDesktop.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using static PeopleCounterDesktop.ViewModels.BuildingSensorsViewModel;

namespace PeopleCounterDesktop.ViewModels;

public class BuildingsViewModel
{
    private readonly PeopleCounterApiService _api;
    private readonly PeopleCounterSignalRService _signalR;
    private bool _initialized;

    public ICommand ResetBuildingCommand =>
    new Command<BuildingSummary>(async building =>
    {

        if (!IsAdmin || building == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Reset",
            $"Reset all devices in '{building.Building}'?",
            "Reset",
            "Cancel");

        if (!confirm)
            return;


        await _api.ResetBuilding(building.Building);

    });


    public ObservableCollection<BuildingSummary> Buildings { get; } = new();

    public ICommand OpenBuildingCommand => new Command<string>(OpenBuilding);

    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set => SetProperty(ref _isAdmin, value);
    }




    public BuildingsViewModel(
        PeopleCounterApiService api,
        PeopleCounterSignalRService signalR)
    {
        _api = api;
        _signalR = signalR;
        IsAdmin = false;
    }

    private async Task LoadUserRoleAsync()
    {
        var cookie = await AuthCookieStore.GetCookie();
        if (string.IsNullOrEmpty(cookie))
        {
            IsAdmin = false;
            return;
        }

        var me = await ApiClient.Client
            .GetFromJsonAsync<UserInfo>("auth/me");

        IsAdmin = me?.Roles.Contains("Admin") == true;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) 
            return;

        _initialized = true;

        await LoadUserRoleAsync();

        Buildings.Clear();
        var data = await _api.GetBuildings();
        foreach (var b in data)
            Buildings.Add(b);

        await _signalR.StartAsync();
        await _signalR.JoinDashboard();

        _signalR.Connection.On<List<BuildingSummary>>(
               "BuildingSummaryUpdated",
               updates =>
               {
                   MainThread.BeginInvokeOnMainThread(() =>
                   {
                       foreach (var u in updates)
                       {
                           var existing = Buildings
                               .FirstOrDefault(x => x.Building == u.Building);

                           if (existing == null)
                               Buildings.Add(u);
                           else
                           {
                               existing.TotalIn = u.TotalIn;
                               existing.TotalOut = u.TotalOut;
                               existing.TotalCapacity = u.TotalCapacity;
                           }
                       }
                   });
               });
    }

    private string _building = string.Empty;

    public void SetBuilding(string building)
    {
        _building = building;
    }

    private async void OpenBuilding(string building)
    {
        await Shell.Current.GoToAsync(nameof(BuildingSensorsPage),
            new Dictionary<string, object> { ["Building"] = building });
    }



    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetProperty<T>(
        ref T backingStore,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));

        return true;
    }

}
