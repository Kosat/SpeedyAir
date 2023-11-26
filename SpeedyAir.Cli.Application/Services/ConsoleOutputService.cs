using SpeedyAir.Models;
using SpeedyAir.Services.Interfaces;

namespace SpeedyAir.Services;

internal class ConsoleOutputService : IOutputService
{
    private readonly TextWriter _consoleWriter;

    public ConsoleOutputService() : this(Console.Out) { }

    public ConsoleOutputService(TextWriter consoleWriter)
        => _consoleWriter = consoleWriter;

    public Task WriteFlightItineraryAsync(DeliveryOrder order, FlightSchedule? flightSchedule = null)
        => _consoleWriter.WriteLineAsync(
            flightSchedule != null
            ? $"order: {order.Name}, flightNumber: {flightSchedule.FlightNumber}, departure: {flightSchedule.Departure}, arrival: {flightSchedule.Arrival}, day: {flightSchedule.Day}"
            : $"order: {order.Name}, flightNumber: not scheduled");

    public Task WriteFlightScheduleAsync(FlightSchedule flightSchedule)
        => _consoleWriter.WriteLineAsync($"Flight: {flightSchedule.FlightNumber}, departure: {flightSchedule.Departure}, arrival: {flightSchedule.Arrival}, day: {flightSchedule.Day}");
}
