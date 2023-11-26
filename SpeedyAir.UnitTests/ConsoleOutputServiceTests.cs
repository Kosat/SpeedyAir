using SpeedyAir.Models;
using SpeedyAir.Services;

namespace SpeedyAir.UnitTests;

public class ConsoleOutputServiceTests
{
    [Fact]
    public async Task WriteFlightItineraryAsync_ShouldWriteCorrectMessage_WhenFlightScheduleIsNotNull()
    {
        // ARRANGE
        var mockConsoleWriter = new Mock<TextWriter>();
        var service = new ConsoleOutputService(mockConsoleWriter.Object);
        var order = new DeliveryOrder { Name = "Order1" };
        var flightSchedule = new FlightSchedule
        {
            FlightNumber = 123,
            Departure = "YUL",
            Arrival = "YYZ",
            Day = 1
        };

        // ACT
        await service.WriteFlightItineraryAsync(order, flightSchedule);

        // ASSERT
        mockConsoleWriter.Verify(m => m.WriteLineAsync(
            "order: Order1, flightNumber: 123, departure: YUL, arrival: YYZ, day: 1"), Times.Once);
    }

    [Fact]
    public async Task WriteFlightItineraryAsync_ShouldWriteNotScheduled_WhenFlightScheduleIsNull()
    {
        // ARRANGE
        var mockConsoleWriter = new Mock<TextWriter>();
        var service = new ConsoleOutputService(mockConsoleWriter.Object);
        var order = new DeliveryOrder { Name = "Order1" };

        // ACT
        await service.WriteFlightItineraryAsync(order);

        // ASSERT
        mockConsoleWriter.Verify(m => m.WriteLineAsync(
            "order: Order1, flightNumber: not scheduled"), Times.Once);
    }

    [Fact]
    public async Task WriteFlightScheduleAsync_ShouldWriteCorrectMessage()
    {
        // ARRANGE
        var mockConsoleWriter = new Mock<TextWriter>();
        var service = new ConsoleOutputService(mockConsoleWriter.Object);
        var flightSchedule = new FlightSchedule
        {
            FlightNumber = 123,
            Departure = "YUL",
            Arrival = "YYZ",
            Day = 1
        };

        // ACT
        await service.WriteFlightScheduleAsync(flightSchedule);

        // ASSERT
        mockConsoleWriter.Verify(m => m.WriteLineAsync(
            "Flight: 123, departure: YUL, arrival: YYZ, day: 1"), Times.Once);
    }
}
