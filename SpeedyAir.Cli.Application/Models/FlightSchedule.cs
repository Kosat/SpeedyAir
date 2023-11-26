using System.Text.Json.Serialization;

namespace SpeedyAir.Models;

internal record FlightSchedule
{
    [JsonPropertyName("day")]
    public int Day { get; init; }

    [JsonPropertyName("flight")]
    public int FlightNumber { get; init; }

    [JsonPropertyName("departure")]
    public string Departure { get; init; } = default!;

    [JsonPropertyName("arrival")]
    public string Arrival { get; init; } = default!;
}
