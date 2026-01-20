using Microsoft.Maui.Controls;
using PeopleCounterDesktop.Services;
using PeopleCounterDesktop.ViewModels;

namespace PeopleCounterDesktop.Views;

[QueryProperty(nameof(Building), "Building")]
public partial class BuildingSensorsPage : ContentPage
{
    private readonly PeopleCounterApiService _api;
    private readonly PeopleCounterSignalRService _signalR;
    private BuildingSensorsViewModel _vm;
    private readonly IServiceProvider _services;
    public string Building { get; set; }

    public BuildingSensorsPage(
        PeopleCounterApiService api,
        PeopleCounterSignalRService signalR, IServiceProvider services)
    {
        InitializeComponent();
        _api = api;
        _signalR = signalR;
        _services = services;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _vm = _services.GetRequiredService<BuildingSensorsViewModel>();
        _vm.SetBuilding(Building);
        BindingContext = _vm;

        await _vm.InitializeAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        try
        {
            if (_signalR.IsConnected)
            {
                await _signalR.LeaveBuilding(Building);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR Leave skipped: {ex.Message}");
        }
    }

}
