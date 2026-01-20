using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PeopleCounterDesktop.Models;

public class BuildingSummary : INotifyPropertyChanged
{
    private int _totalIn;
    private int _totalOut;
    private int _totalCapacity;

    public string Building { get; set; }

    public int TotalIn
    {
        get => _totalIn;
        set => SetProperty(ref _totalIn, value);
    }

    public int TotalOut
    {
        get => _totalOut;
        set => SetProperty(ref _totalOut, value);
    }

    public int TotalCapacity
    {
        get => _totalCapacity;
        set => SetProperty(ref _totalCapacity, value);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected void SetProperty<T>(
        ref T backingStore,
        T value,
        [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return;

        backingStore = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
