using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Input;
public class PeopleCounterModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    void Notify([CallerMemberName] string? p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

    int _in, _out, _cap;
    DateTime _time;

    [JsonPropertyName("deviceId")]
    public string Device { get; set; } = "";
    [JsonPropertyName("location")]
    public string Location { get; set; } = "";

    [JsonPropertyName("sublocation")]
    public string SubLocation { get; set; } = "";

    [JsonPropertyName("inCount")]
    public int Total_IN { get => _in; set { _in = value; Notify(); } }

    [JsonPropertyName("outCount")]
    public int Total_Out { get => _out; set { _out = value; Notify(); } }

    [JsonPropertyName("capacity")]
    public int Capacity { get => _cap; set { _cap = value; Notify(); } }

    [JsonPropertyName("eventTime")]
    public DateTime TimeStamp { get => _time; set { _time = value; Notify(); } }

    public ICommand ResetCommand { get; set; }

}