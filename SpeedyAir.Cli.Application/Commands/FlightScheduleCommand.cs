using System.Diagnostics.CodeAnalysis;
using CommandLine;
using SpeedyAir.Exceptions;

namespace SpeedyAir.Commands;

[SuppressMessage("ReSharper", "UnusedType.Global")]
[Verb("list-schedules", HelpText = "List schedules found in input file")]
public class FlightScheduleCommand
{

    [Option('f', "input-file", Required = true, HelpText = @"Input file name or path")]
    public string InputFile { get; set; } = default!;

    public void Validate()
    {
        if (!File.Exists(InputFile))
        {
            throw new CliArgumentValidationException($"Invalid schedule file path.");
        }
    }

}
