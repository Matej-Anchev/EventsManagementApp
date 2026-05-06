using Domain.Models;

namespace Service.Interface;

public interface IVenueService
{
    Task<List<Venue>> GetAllAsync();
    Task<Venue?> GetByIdAsync(Guid id);
    Task<Venue> GetByIdNotNull(Guid id);
    Task<Venue> InsertAsync(string name, string address, string city, string country, string? zipCode,
        int totalCapacity);

    Task<Venue> UpdateAsync(Guid id, string name, string address, string city, string country, string? zipCode,
        int totalCapacity);

    Task<Venue> DeleteByIdAsync(Guid id);
    Task<Venue?> GetByNameAndCityAsync(string name, string city);
}