using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class VenueService : IVenueService
{
    private readonly IRepository<Venue> _repository;

    public VenueService(IRepository<Venue> repository)
    {
        _repository = repository;
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        var venues = await _repository.GetAllAsync(x => x);
        return venues.ToList();
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(x => x, x => x.Id == id);
    }

    public async Task<Venue> GetByIdNotNull(Guid id)
    {
        var venue = await GetByIdAsync(id);
        if (venue == null)
            throw new InvalidOperationException($"Venue with id {id} not found");

        return venue;
    }

    public async Task<Venue> InsertAsync(string name, string address, string city, string country, string? zipCode,
        int totalCapacity)
    {
        var venueToAdd = new Venue()
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            ZipCode = zipCode,
            TotalCapacity = totalCapacity
        };

        return await _repository.InsertAsync(venueToAdd);
    }

    public async Task<Venue> UpdateAsync(Guid id, string name, string address, string city, string country,
        string? zipCode,
        int totalCapacity)
    {
        var venueToUpdate = await GetByIdNotNull(id);
        venueToUpdate.Name = name;
        venueToUpdate.Address = address;
        venueToUpdate.City = city;
        venueToUpdate.Country = country;
        venueToUpdate.ZipCode = zipCode;
        venueToUpdate.TotalCapacity = totalCapacity;

        return await _repository.UpdateAsync(venueToUpdate);
    }

    public async Task<Venue> DeleteByIdAsync(Guid id)
    {
        var venueToDelete = await GetByIdNotNull(id);
        return await _repository.DeleteAsync(venueToDelete);
    }

    public async Task<Venue?> GetByNameAndCityAsync(string name, string city)
    {
        return await _repository.Get(
            selector: x => x,
            predicate: x => x.Name == name && x.City == city
        );
    }
}