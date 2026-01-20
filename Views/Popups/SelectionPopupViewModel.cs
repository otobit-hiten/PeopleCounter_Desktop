namespace PeopleCounterDesktop.Views.Popups;

public class SelectionPopupViewModel
{
    public IEnumerable<string> Items { get; }

    public SelectionPopupViewModel(IEnumerable<string> items)
    {
        Items = items;
    }
}
