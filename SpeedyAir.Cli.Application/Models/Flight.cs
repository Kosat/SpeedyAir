namespace SpeedyAir.Models;

internal record Flight(FlightSchedule FlightSchedule, uint Capacity)
{
    public uint Capacity { get; private set; } = Capacity;

    public uint DecrementCapacity() => --Capacity;
}
