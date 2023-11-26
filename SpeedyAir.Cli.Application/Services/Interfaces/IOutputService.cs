using SpeedyAir.Models;

namespace SpeedyAir.Services.Interfaces;

internal interface IOutputService
{
    Task WriteFlightScheduleAsync(FlightSchedule flightSchedule);
    Task WriteFlightItineraryAsync(DeliveryOrder order, FlightSchedule? flightSchedule = null);
}
