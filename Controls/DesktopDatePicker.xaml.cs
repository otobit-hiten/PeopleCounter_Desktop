namespace PeopleCounterDesktop.Controls;

public partial class DesktopDatePicker : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(DesktopDatePicker));

    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(DesktopDatePicker), DateTime.Today);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DesktopDatePicker()
    {
        InitializeComponent();
    }

    private void OnTapped(object sender, EventArgs e)
    {
        Picker.Focus();
    }
}
