using Domain.Models;

namespace Repository.Interface;

public interface IVenueRepository
{
    Task BulkInsertOrUpdateVenuesAsync(List<Venue> venues);
    Task BulkInsertOrUpdateSectionsAsync(List<Section> sections);
    Task BulkInsertOrUpdateSeatsAsync(List<Seat> seats);
}