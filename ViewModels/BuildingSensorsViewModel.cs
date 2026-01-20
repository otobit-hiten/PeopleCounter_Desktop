using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.ApplicationModel;
using PeopleCounterDesktop.Models;
using PeopleCounterDesktop.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PeopleCounterDesktop.ViewModels
{
    public class BuildingSensorsViewModel: INotifyPropertyChanged
    {
        private readonly PeopleCounterApiService _api;
        private readonly PeopleCounterSignalRService _signalR;
        private bool _signalRInitialized;
        private string _building = string.Empty;
        public ObservableCollection<PeopleCounterModel> Sensors { get; } = new();


        public string Building
        {
            get => _building;
            private set => SetProperty(ref _building, value);
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged();
                }
            }
        }

        public BuildingSensorsViewModel(PeopleCounterApiService api, PeopleCounterSignalRService signalR)
        {
            _api = api;
            _signalR = signalR;
            IsAdmin = false;
        }

        public void SetBuilding(string building)
        {
            _building = building;
        }

        public async Task InitializeAsync()
        {
            if (_signalRInitialized)
                return;

            _signalRInitialized = true;

            await LoadUserRoleAsync();
            await LoadSensorsAsync();
            await SetupSignalRAsync();
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

        private async Task LoadSensorsAsync()
        {
            Sensors.Clear();

            var data = await _api.GetSensors(Building);
            foreach (var sensor in data)
            {
                sensor.ResetCommand = new Command(() =>
                    ResetDevice(sensor).FireAndForget());

                Sensors.Add(sensor);
            }
        }

        private async Task SetupSignalRAsync()
        {
            await _signalR.StartAsync();
            await _signalR.JoinBuilding(Building);

            _signalR.Connection.On<PeopleCounterModel>(
                "SensorUpdated",
                sensor =>
                {
                    if (sensor.Location != Building) return;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var existing = Sensors
                            .FirstOrDefault(x => x.Device == sensor.Device);

                        if (existing == null)
                            Sensors.Add(sensor);
                        else
                        {
                            existing.Total_IN = sensor.Total_IN;
                            existing.Total_Out = sensor.Total_Out;
                            existing.Capacity = sensor.Capacity;
                            existing.TimeStamp = sensor.TimeStamp;
                        }
                    });
                });

            _signalR.Connection.On<string>(
                "BuildingReset",
                building =>
                {
                    if (building != Building) return;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var sensor in Sensors)
                        {
                            sensor.Total_IN = 0;
                            sensor.Total_Out = 0;
                            sensor.Capacity = 0;
                            sensor.TimeStamp = DateTime.Now;
                        }
                    });
                });


            _signalR.Connection.On<string>(
                "DeviceReset",
                deviceId =>
                {
                    var sensor = Sensors.FirstOrDefault(x => x.Device == deviceId);
                    if (sensor == null) return;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        sensor.Total_IN = 0;
                        sensor.Total_Out = 0;
                        sensor.Capacity = 0;
                        sensor.TimeStamp = DateTime.Now;
                    });
                });
        }

        public async Task ResetDevice(PeopleCounterModel device)
        {
            if (device == null) return;

            await _api.ResetDevice(device.Device);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                device.Total_IN = 0;
                device.Total_Out = 0;
                device.Capacity = 0;
                device.TimeStamp = DateTime.Now;
            });
        }

        public record UserInfo(string Username, List<string> Roles);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(
            ref T backingStore,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}


public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
       
    }
}
