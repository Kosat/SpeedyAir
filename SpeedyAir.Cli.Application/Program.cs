using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SpeedyAir.Commands;
using SpeedyAir.Commands.Handlers;
using SpeedyAir.Exceptions;
using SpeedyAir.Models;
using SpeedyAir.Repositories;
using SpeedyAir.Repositories.Interfaces;
using SpeedyAir.Services;
using SpeedyAir.Services.Interfaces;

namespace SpeedyAir;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Program
{
    private static async Task<int> Main(string[] args)
    {
        /* NOTES from Kostya:
         *
         * 1. To keep things simple. I put all the classes in one project and overall tried not to over-engineer the code for this 1-2 hour
         * assignment.
         *
         * 2. For this exercise, the project has <Nullable>enable</Nullable> and I (in the context of this exercise) trust compiler on it.
         *
         * 3. To save time, in UnitTests project, I have only covered happy path cases and omitted the scenarios where exceptions are thrown.
         *
         * 4. Airplane capacity is configured in appsettings.json Airplane:DefaultCapacity = 20 .
         *
         * 5. Flight schedules are loaded from a json file, similarly to the orders.
         */
        var cliParserResult = Parser.Default.ParseArguments<FlightScheduleCommand, FlightItineraryCommand>(args);
        IHost host;
        try
        {
            host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configure Serilog
                    Log.Logger = new LoggerConfiguration().ReadFrom
                                .Configuration(context.Configuration)
                                .CreateLogger();

                    services.AddSingleton<IOutputService, ConsoleOutputService>();
                    services.AddSingleton<FlightScheduleCommandHandler>();
                    services.AddSingleton<FlightItineraryCommandHandler>();

                    cliParserResult
                        .WithParsed<FlightScheduleCommand>(options =>
                        {
                            options.Validate();
                            services.AddSingleton<IRepository<FlightSchedule>, ScheduleRepository>(_ => new ScheduleRepository(options.InputFile));
                        })
                        .WithParsed<FlightItineraryCommand>(options =>
                        {
                            options.Validate();
                            services.AddSingleton<IRepository<FlightSchedule>, ScheduleRepository>(_ => new ScheduleRepository(options.ScheduleInputFile));
                            services.AddSingleton<IRepository<DeliveryOrder>, OrderRepository>(_ => new OrderRepository(options.OrdersInputFile));
                        });
                })
                .UseSerilog()
                .Build() ?? throw new Exception("Failed to build the host. CreateDefaultBuilder() unexpectedly returned null.") ;
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Unhandled exception when configuring/building IHost instance.");
            await Console.Error.WriteLineAsync("Unhandled exception when configuring/building IHost instance. Fail fast.");
            throw;
        }

        try
        {
            return await cliParserResult.MapResult(
                (FlightScheduleCommand options) => host.Services.GetRequiredService<FlightScheduleCommandHandler>().Handle(options),
                (FlightItineraryCommand options) => host.Services.GetRequiredService<FlightItineraryCommandHandler>().Handle(options),
                _ => Task.FromResult(1)
            );
        }
        catch (PersistenceLayerException ex)
        {
            Log.Logger.Error(ex, "Error when loading data");
            await Console.Error.WriteLineAsync("Failed to load input data. Please check the correctness of the input data and try again.");
            return 1;
        }
        catch (ApplicationLogicException ex)
        {
            Log.Logger.Error(ex, "Error when handling CLI command");
            await Console.Error.WriteLineAsync($"Command {ex.CommandType.Name} execution failed");
            return 1;
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Unhandled exception when handling a CLI command");
            await Console.Error.WriteLineAsync("Unhandled exception when handling a CLI command. Fail fast.");
            throw;
        }
    }
}
