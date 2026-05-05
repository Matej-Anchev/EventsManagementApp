using Domain.Models;
using Repository.Interface;

namespace Repository.Implementation;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    public VenueRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task BulkInsertOrUpdateVenuesAsync(List<Venue> venues)
    {
        await _context.BulkInsertOrUpdateVenueAsync(venues);
    }

    public async Task BulkInsertOrUpdateSectionsAsync(List<Section> sections)
    {
        await _context.BulkInsertOrUpdateSectionsAsync(sections);
    }

    public async Task BulkInsertOrUpdateSeatsAsync(List<Seat> seats)
    {
        await _context.BuldInsertOrUpdateSeatsAsync(seats);
    }
}