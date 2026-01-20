using CommunityToolkit.Maui.Views;

namespace PeopleCounterDesktop.Views.Popups;

public partial class SelectionPopup : Popup
{
    public SelectionPopup(IEnumerable<string> items)
    {
        InitializeComponent();
        BindingContext = new SelectionPopupViewModel(items);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.FirstOrDefault() is string selected)
        {
            Close(selected); // returns value
        }
    }
}
