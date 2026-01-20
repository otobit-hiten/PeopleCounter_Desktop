using System.Windows.Input;

namespace PeopleCounterDesktop.Controls;

public partial class DesktopDropdown : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(DesktopDropdown));

    public static readonly BindableProperty DisplayTextProperty =
        BindableProperty.Create(nameof(DisplayText), typeof(string), typeof(DesktopDropdown));

    public static readonly BindableProperty OpenCommandProperty =
        BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(DesktopDropdown));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public ICommand OpenCommand
    {
        get => (ICommand)GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public DesktopDropdown()
    {
        InitializeComponent();
    }
}
