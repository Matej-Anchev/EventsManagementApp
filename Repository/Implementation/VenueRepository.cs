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
        foreach (var venue in venues)
        {
            var existing = await _context.Venues.FindAsync(venue.Id);
            if (existing == null)
                await _context.Venues.AddAsync(venue);
            else
                _context.Entry(existing).CurrentValues.SetValues(venue);
        }

        await _context.SaveChangesAsync();
    }

    public async Task BulkInsertOrUpdateSectionsAsync(List<Section> sections)
    {
        foreach (var section in sections)
        {
            var existing = await _context.Sections.FindAsync(section.Id);
            if (existing == null)
                await _context.Sections.AddAsync(section);
            else
                _context.Entry(existing).CurrentValues.SetValues(section);
        }

        await _context.SaveChangesAsync();
    }

    public async Task BulkInsertOrUpdateSeatsAsync(List<Seat> seats)
    {
        foreach (var seat in seats)
        {
            var existing = await _context.Seats.FindAsync(seat.Id);
            if (existing == null)
                await _context.Seats.AddAsync(seat);
            else
                _context.Entry(existing).CurrentValues.SetValues(seat);
        }

        await _context.SaveChangesAsync();
    }
}