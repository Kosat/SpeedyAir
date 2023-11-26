using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpeedyAir.Exceptions;
using SpeedyAir.Models;
using SpeedyAir.Repositories.Interfaces;
using SpeedyAir.Services;
using SpeedyAir.Services.Interfaces;

namespace SpeedyAir.Commands.Handlers;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class FlightItineraryCommandHandler
{
    private readonly ILogger<FlightItineraryCommandHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IOutputService _outputService;
    private readonly IRepository<DeliveryOrder> _orderRepository;
    private readonly IRepository<FlightSchedule> _scheduleRepository;

    public FlightItineraryCommandHandler(
        ILogger<FlightItineraryCommandHandler> logger,
        IConfiguration configuration,
        IOutputService outputService,
        IRepository<DeliveryOrder> orderRepository,
        IRepository<FlightSchedule> scheduleRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _outputService = outputService;
        _orderRepository = orderRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<int> Handle(FlightItineraryCommand options)
    {
        _logger.LogDebug("Start handling {Command} for files {ScheduleFile} {OrdersFile}", nameof(FlightItineraryCommand), options.ScheduleInputFile, options.OrdersInputFile);

        var scheduledFlights = await _scheduleRepository.GetAllAsync();

        var scheduledFlightsDestinationIndex = CreateScheduledFlightsDestinationIndex(scheduledFlights);

        var orders = await _orderRepository.GetAllAsync();

        await PrintOutFlightItineraries(orders, scheduledFlightsDestinationIndex);

        _logger.LogInformation("Done printing out generated flight itineraries");

        return 0;
    }

    private async Task PrintOutFlightItineraries(IEnumerable<DeliveryOrder> orders, Dictionary<string, FlightSchedulesGroupedByDestination> scheduledFlightsDestinationIndex)
    {
        foreach (DeliveryOrder order in orders)
        {
            if (scheduledFlightsDestinationIndex.TryGetValue(order.Destination, out var schedulesGroupedByDestination))
            {
                if (schedulesGroupedByDestination.TryAllocateOrder(out Flight? flight))
                {
                    await _outputService.WriteFlightItineraryAsync(order, flight!.FlightSchedule);
                }
                else
                {
                    await _outputService.WriteFlightItineraryAsync(order);
                }
            }
            else
            {
                await _outputService.WriteFlightItineraryAsync(order);
            }
        }
    }

    private Dictionary<string, FlightSchedulesGroupedByDestination> CreateScheduledFlightsDestinationIndex(IEnumerable<FlightSchedule> scheduledFlights)
    {
        var defaultAirplaneCapacity = _configuration.GetValue<uint>("Airplane:DefaultCapacity");
        var scheduledFlightsDestinationIndex = new Dictionary<string /*destination e.g. YYC*/, FlightSchedulesGroupedByDestination>();
        foreach (var scheduledFlight in scheduledFlights)
        {
            try
            {
                var flight = new Flight(scheduledFlight, defaultAirplaneCapacity);
                if (scheduledFlightsDestinationIndex.TryGetValue(scheduledFlight.Arrival, out var schedulesGroupedByDestination))
                {
                    schedulesGroupedByDestination.AddSchedule(flight);
                }
                else
                {
                    scheduledFlightsDestinationIndex.Add(scheduledFlight.Arrival, new FlightSchedulesGroupedByDestination(flight));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationLogicException(typeof(FlightItineraryCommand), $"Failed to process flight {scheduledFlight.FlightNumber} when building the ScheduledFlightsDestinationIndex" ,ex);
            }
        }
        _logger.LogInformation("Successfully created the flights destination index ");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            if (scheduledFlightsDestinationIndex.Count == 0)
            {
                _logger.LogDebug("Destination index is empty");
            }
            else
            {
                foreach (var flightSchedulesGroupedByDestination in scheduledFlightsDestinationIndex)
                {
                    _logger.LogDebug("Destination index day={Day} schedules count={Count}", flightSchedulesGroupedByDestination.Key, flightSchedulesGroupedByDestination.Value.Schedules.Count);
                }
            }
        }

        return scheduledFlightsDestinationIndex;
    }
}
