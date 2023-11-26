using System.Diagnostics.CodeAnalysis;
using CommandLine;
using SpeedyAir.Exceptions;

namespace SpeedyAir.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
[Verb("generate-itineraries", HelpText = "List schedules found in input file")]
public class FlightItineraryCommand
{
    [Option('s', "schedule-file", Required = true, HelpText = @"Input json file with flight schedules")]
    public string ScheduleInputFile { get; set; } = default!;

    [Option('o', "orders-file", Required = true, HelpText = @"Input json file with orders")]
    public string OrdersInputFile { get; set; } = default!;

    public void Validate()
    {
        if (!File.Exists(ScheduleInputFile))
        {
            throw new CliArgumentValidationException($"Invalid schedule file path");
        }

        if (!File.Exists(OrdersInputFile))
        {
            throw new CliArgumentValidationException($"Invalid orders file path");
        }
    }

}
