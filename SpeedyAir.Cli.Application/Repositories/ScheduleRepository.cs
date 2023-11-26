using SpeedyAir.Models;
using System.Text.Json;
using SpeedyAir.Exceptions;

namespace SpeedyAir.Repositories;

internal class ScheduleRepository : BaseRepository<FlightSchedule>
{
    public ScheduleRepository(string filePath) : base(filePath)
    {
    }

    protected override async Task<List<FlightSchedule>> LoadData()
    {
        try
        {
            await using FileStream scheduleFileStream = File.OpenRead(FilePath);
            return await JsonSerializer.DeserializeAsync<List<FlightSchedule>>(scheduleFileStream) ?? new List<FlightSchedule>();
        }
        catch (Exception ex)
        {
            throw new PersistenceLayerException($"Failed to load flight schedules data from {FilePath}", ex);
        }
    }
}
