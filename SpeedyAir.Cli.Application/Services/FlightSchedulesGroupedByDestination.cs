using SpeedyAir.Models;

namespace SpeedyAir.Services;

internal class FlightSchedulesGroupedByDestination
{
    private readonly List<Flight> _schedules = new();
    private int? _currentNonFullScheduleIndex;
    private bool _isAllScheduledFlightsAreFull;

    public IReadOnlyList<Flight> Schedules => _schedules;

    public FlightSchedulesGroupedByDestination(params Flight[] schedules)
    {
        _schedules.AddRange(schedules);
        _currentNonFullScheduleIndex = 0;
    }

    public bool TryAllocateOrder(out Flight? flight)
    {
        if (_isAllScheduledFlightsAreFull || _currentNonFullScheduleIndex is null)
        {
            flight = null;
            return false;
        }

        flight = Schedules[_currentNonFullScheduleIndex.Value];
        var currentCapacity = flight.DecrementCapacity();
        if (currentCapacity != 0)
        {
            return true;
        }
        if (_currentNonFullScheduleIndex == Schedules.Count - 1)
        {
            // All the schedules are full
            _isAllScheduledFlightsAreFull = true;
            _currentNonFullScheduleIndex = null;
        }
        else
        {
            // Current flight schedule has been depleted. Move to the next flight schedule that has capacity available.
            _currentNonFullScheduleIndex++;
        }

        return true;
    }

    public void AddSchedule(Flight schedule)
    {
        _schedules.Add(schedule);
        _isAllScheduledFlightsAreFull = false;
    }
}
