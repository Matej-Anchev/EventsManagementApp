using Domain.ExternalModels;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class LegacyVenueRepository : ILegacyVenueRepository
{
    private readonly LegacyApplicationDbContext _dbContext;

    public LegacyVenueRepository(LegacyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Venue>> GetVenuesModifiedSinceAsync(DateTime since)
    {
        var legacy = await _dbContext.Venues.Where(x => x.LastModified >= since).ToListAsync();

        return legacy.Select(x => new Venue()
        {
            Id = GuidHelper.FromLegacyId("Venue", x.VenueId),
            Name = x.Name,
            Address = x.Address,
            City = x.City,
            Country = x.Country,
            TotalCapacity = x.TotalCapacity,
            ZipCode = x.ZipCode
        }).ToList();
    }

    public async Task<List<Venue>> GetSectionsModifiedSinceAsync(DateTime since)
    {
        var legacy = await _dbContext.Venues.Where(x => x.LastModified >= since).ToListAsync();

        return legacy.Select(x => new Venue()
        {
            Id = GuidHelper.FromLegacyId("Venue", x.VenueId),
            Name = x.Name,
            Address = x.Address,
            City = x.City,
            Country = x.Country,
            TotalCapacity = x.TotalCapacity,
            ZipCode = x.ZipCode,
        }).ToList();
    }

    public async Task<List<Seat>> GetSeatsModifiedSinceAsync(DateTime since)
    {
        var legacySeats = await _dbContext.Seats
            .AsNoTracking()
            .Where(s => s.LastModified > since)
            .ToListAsync();

        return legacySeats.Select(ls => new Seat
        {
            Id = GuidHelper.FromLegacyId("Seat", ls.SeatId),
            SectionId = GuidHelper.FromLegacyId("Section", ls.SectionId),
            Row = ls.Row,
            Number = ls.Number,
            Label = "",
            IsAccessible = ls.IsAccessible
        }).ToList();
    }
}