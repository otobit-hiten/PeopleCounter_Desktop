using System;
using System.Text.Json.Serialization;

namespace PeopleCounterDesktop.Models;

public class PeopleCounterSignalRDto
{
    [JsonPropertyName("deviceId")]
    public string Device { get; set; } = "";

    [JsonPropertyName("location")]
    public string Location { get; set; } = "";

    [JsonPropertyName("sublocation")]
    public string SubLocation { get; set; } = "";

    [JsonPropertyName("inCount")]
    public int Total_IN { get; set; }

    [JsonPropertyName("outCount")]
    public int Total_Out { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }

    [JsonPropertyName("eventTime")]
    public DateTime TimeStamp { get; set; }
}
