using System.Text.Json;
using SpeedyAir.Exceptions;
using SpeedyAir.Models;

namespace SpeedyAir.Repositories;

internal class OrderRepository : BaseRepository<DeliveryOrder>
{
    public OrderRepository(string filePath) : base(filePath)
    {
    }

    protected override async Task<List<DeliveryOrder>> LoadData()
    {
        try
        {
            var orders = new List<DeliveryOrder>();
            await using FileStream fileStream = File.OpenRead(FilePath);
            using JsonDocument parsedJsonRootDocument = await JsonDocument.ParseAsync(fileStream);
            foreach (var element in parsedJsonRootDocument.RootElement.EnumerateObject())
            {
                var parsedOrder = new DeliveryOrder(element.Name, element.Value.GetProperty("destination").GetString()
                                                                  ?? throw new JsonException("Failed to parse json destination property value"));
                orders.Add(parsedOrder);
            }

            return orders;
        }
        catch (Exception ex)
        {
            throw new PersistenceLayerException($"Failed to load orders data from {FilePath}", ex);
        }
    }
}
