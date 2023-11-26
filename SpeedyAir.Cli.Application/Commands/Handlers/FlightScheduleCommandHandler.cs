using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using SpeedyAir.Exceptions;
using SpeedyAir.Models;
using SpeedyAir.Services.Interfaces;
using SpeedyAir.Repositories.Interfaces;

namespace SpeedyAir.Commands.Handlers;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class FlightScheduleCommandHandler
{
    private readonly ILogger<FlightScheduleCommandHandler> _logger;
    private readonly IOutputService _outputService;
    private readonly IRepository<FlightSchedule> _scheduleRepository;

    public FlightScheduleCommandHandler(
        ILogger<FlightScheduleCommandHandler> logger,
        IOutputService outputService,
        IRepository<FlightSchedule> scheduleRepository)
    {
        _logger = logger;
        _outputService = outputService;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<int> Handle(FlightScheduleCommand options)
    {
        _logger.LogDebug("Start handling {Command} for file {InputFile}", nameof(FlightScheduleCommand), options.InputFile);

        var scheduledFlights = await _scheduleRepository.GetAllAsync();

        if(_logger.IsEnabled(LogLevel.Trace))
        {
            // ReSharper disable once PossibleMultipleEnumeration | iterate multiple times and lose performance when in trace mode
            _logger.LogTrace("Successfully loaded {Count} flight schedules form input file", scheduledFlights?.Count() ?? 0);
        }

        foreach (var flight in scheduledFlights)
        {
            try
            {
                await _outputService.WriteFlightScheduleAsync(flight);
            }
            catch (Exception ex)
            {
                throw new ApplicationLogicException(typeof(FlightScheduleCommand), $"Failed to print out the flight {flight.FlightNumber} ", ex);
            }
        }

        _logger.LogInformation("Done printing out the flight schedules");

        return 0;
    }

}
