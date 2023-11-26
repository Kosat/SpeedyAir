using Microsoft.Extensions.Logging;
using SpeedyAir.Commands;
using SpeedyAir.Commands.Handlers;
using SpeedyAir.Models;
using SpeedyAir.Repositories.Interfaces;
using SpeedyAir.Services.Interfaces;

namespace SpeedyAir.UnitTests;

public class FlightScheduleCommandHandlerTests
{
    private readonly FlightScheduleCommandHandler _sut;

    private readonly Mock<ILogger<FlightScheduleCommandHandler>> _loggerMock = new();
    private readonly Mock<IOutputService> _outputServiceMock = new();
    private readonly Mock<IRepository<FlightSchedule>> _scheduleRepositoryMock = new();

    public FlightScheduleCommandHandlerTests()
        => _sut = new FlightScheduleCommandHandler(_loggerMock.Object, _outputServiceMock.Object, _scheduleRepositoryMock.Object);

    [Fact]
    public async Task Should_Output_All_Schedules_As_Is()
    {
        // ARRANGE
        var testFlightSchedules = new List<FlightSchedule>()
        {
            new() { FlightNumber = 1, Day = 1, Departure = "YUL", Arrival = "YYZ" },
            new() { FlightNumber = 2, Day = 1, Departure = "YUL", Arrival = "YYC" },
            new() { FlightNumber = 3, Day = 2, Departure = "YUL", Arrival = "YYZ" },
            new() { FlightNumber = 4, Day = 3, Departure = "YUL", Arrival = "YVR" }
        };
        _scheduleRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(testFlightSchedules);

        List<FlightSchedule> actualOutput = new();
        _outputServiceMock.Setup(r => r.WriteFlightScheduleAsync(It.IsAny<FlightSchedule>()))
            .Callback((FlightSchedule schedule) => actualOutput.Add(schedule));

        // ACT
        var options = new FlightScheduleCommand { InputFile = "abc.json" };
        int exitCode = await _sut.Handle(options);

        // ASSERT
        exitCode.Should().Be(0);
        actualOutput.Should().HaveCount(4).And.ContainInOrder(testFlightSchedules);
    }

    [Fact]
    public async Task Should_Produce_No_Output_When_No_Data()
    {
        // ARRANGE
        _scheduleRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<FlightSchedule>());

        List<FlightSchedule> actualOutput = new();
        _outputServiceMock.Setup(x => x.WriteFlightScheduleAsync(It.IsAny<FlightSchedule>()))
            .Callback((FlightSchedule schedule) => actualOutput.Add(schedule));

        // ACT
        var options = new FlightScheduleCommand { InputFile = "abc.json" };
        int exitCode = await _sut.Handle(options);

        // ASSERT
        exitCode.Should().Be(0);
        actualOutput.Should().BeEmpty();
    }
}
