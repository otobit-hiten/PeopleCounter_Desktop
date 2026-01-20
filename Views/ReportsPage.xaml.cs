using PeopleCounterDesktop.ViewModels;

namespace PeopleCounterDesktop.Views;

public partial class ReportsPage : ContentPage
{
    private ReportViewModel _viewModel;
    public ReportsPage(ReportViewModel vm)
	{
		InitializeComponent();
        BindingContext = _viewModel = vm;

        // Subscribe to property changes to update button text
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ReportViewModel.IsLineChartVisible))
            {
                UpdateToggleButtonText();
            }
        };

        // Set initial button text
        UpdateToggleButtonText();
    }

    private void UpdateToggleButtonText()
    {
        ToggleButton.Text = _viewModel.IsLineChartVisible ? "Bar Chart" : "Line Chart";
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReportViewModel vm)
            await vm.Init();
    }
}