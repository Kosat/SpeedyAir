using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpeedyAir.Commands;
using SpeedyAir.Commands.Handlers;
using SpeedyAir.Models;
using SpeedyAir.Repositories.Interfaces;
using SpeedyAir.Services.Interfaces;

namespace SpeedyAir.UnitTests;

public class FlightItineraryCommandHandlerTests
{
    private readonly FlightItineraryCommandHandler _sut;

    private readonly Mock<ILogger<FlightItineraryCommandHandler>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IOutputService> _outputServiceMock = new();
    private readonly Mock<IRepository<FlightSchedule>> _scheduleRepositoryMock = new();
    private readonly Mock<IRepository<DeliveryOrder>> _orderRepositoryMock = new();

    public FlightItineraryCommandHandlerTests()
        => _sut = new FlightItineraryCommandHandler(_loggerMock.Object, _configurationMock.Object, _outputServiceMock.Object, _orderRepositoryMock.Object, _scheduleRepositoryMock.Object);

    [Fact]
    public async Task Should_Output_All_Itineraries()
    {
        // ARRANGE
        var mockConfSection = new Mock<IConfigurationSection>();
        mockConfSection.SetupGet(m => m.Value).Returns("5");
        _configurationMock.Setup(a => a.GetSection(It.Is<string>(s => s == "Airplane:DefaultCapacity"))).Returns(mockConfSection.Object);

        var testFlightSchedules = new List<FlightSchedule>()
        {
            new() { FlightNumber = 1, Day = 1, Departure = "YUL", Arrival = "YYZ" },
            new() { FlightNumber = 2, Day = 1, Departure = "YUL", Arrival = "YYC" },
            new() { FlightNumber = 3, Day = 2, Departure = "YUL", Arrival = "YYZ" },
            new() { FlightNumber = 4, Day = 3, Departure = "YUL", Arrival = "YVR" }
        };
        _scheduleRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(testFlightSchedules);

        var testDeliveryOrders = new List<DeliveryOrder>()
        {
            new() { Name = "order-001", Destination = "YYZ" },
            new() { Name = "order-002", Destination = "YYZ" },
            new() { Name = "order-003", Destination = "YYR" },
            new() { Name = "order-004", Destination = "YYX" }
        };
        _orderRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(testDeliveryOrders);

        List<(DeliveryOrder order, FlightSchedule? schedule)> actualOutput = new();
        _outputServiceMock.Setup(x => x.WriteFlightItineraryAsync(It.IsAny<DeliveryOrder>(), It.IsAny<FlightSchedule?>()))
            .Callback((DeliveryOrder order, FlightSchedule? schedule) => actualOutput.Add((order, schedule)));

        // ACT
        var options = new FlightItineraryCommand { ScheduleInputFile = "abc.json", OrdersInputFile = "xyz.json" };
        int exitCode = await _sut.Handle(options);

        // ASSERT
        exitCode.Should().Be(0);
        actualOutput.Should().HaveCount(4)
            .And.ContainInOrder(
                new List<(DeliveryOrder order, FlightSchedule? schedule)>
                {
                    (testDeliveryOrders[0], testFlightSchedules[0]),
                    (testDeliveryOrders[1], testFlightSchedules[0]),
                    (testDeliveryOrders[2], null),
                    (testDeliveryOrders[3], null)
                });
    }

    [Fact]
    public async Task Should_Output_No_Itineraries_When_Input_Is_Empty()
    {
        // ARRANGE
        var mockConfSection = new Mock<IConfigurationSection>();
        mockConfSection.SetupGet(m => m.Value).Returns("5");
        _configurationMock.Setup(a => a.GetSection(It.Is<string>(s => s == "Airplane:DefaultCapacity"))).Returns(mockConfSection.Object);

        var testFlightSchedules = Enumerable.Empty<FlightSchedule>();
        _scheduleRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(testFlightSchedules);

        var testDeliveryOrders = Enumerable.Empty<DeliveryOrder>();
        _orderRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(testDeliveryOrders);

        List<(DeliveryOrder order, FlightSchedule? schedule)> actualOutput = new();
        _outputServiceMock.Setup(x => x.WriteFlightItineraryAsync(It.IsAny<DeliveryOrder>(), It.IsAny<FlightSchedule?>()))
            .Callback((DeliveryOrder order, FlightSchedule? schedule) => actualOutput.Add((order, schedule)));

        // ACT
        var options = new FlightItineraryCommand { ScheduleInputFile = "abc.json", OrdersInputFile = "xyz.json" };
        int exitCode = await _sut.Handle(options);

        // ASSERT
        exitCode.Should().Be(0);
        actualOutput.Should().BeEmpty();
    }

}
