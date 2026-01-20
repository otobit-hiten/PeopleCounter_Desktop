using Microsoft.Maui.Controls;
using PeopleCounterDesktop.ViewModels;
namespace PeopleCounterDesktop.Views;

public partial class BuildingsPage : ContentPage
{
    private readonly BuildingsViewModel _vm;
    public BuildingsPage(BuildingsViewModel vm)
    {
		InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}