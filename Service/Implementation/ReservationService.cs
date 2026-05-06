using Domain.Dto.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ReservationService : IReservationService
{
    private readonly IRepository<Reservation> _repository;

    public ReservationService(IRepository<Reservation> repository)
    {
        _repository = repository;
    }

    public async Task<Reservation> InsertAsync(Guid eventId, string userId)
    {
        var reservation = new Reservation()
        {
            EventId = eventId,
            UserId = userId,
            ReservedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.Now.AddDays(10),
            Status = ReservationStatus.Pending
        };

        return await _repository.InsertAsync(reservation);
    }

    public async Task<Reservation> UpdateAsync(Guid reservationId, Guid eventId)
    {
        var reservation = await GetByIdNotNullAsync(reservationId);
        reservation.EventId = eventId;

        return await _repository.UpdateAsync(reservation);
    }

    public async Task<Reservation> ConfirmAsync(Guid reservationId)
    {
        var reservation = await GetByIdNotNullAsync(reservationId);
        reservation.Status = ReservationStatus.Confirmed;
        return await _repository.UpdateAsync(reservation);
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(
            selector: x => x,
            include: x => x.Include(r => r.Event)
                .Include(r => r.User));
        return result.ToList();
    }

    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Reservation> GetByIdNotNullAsync(Guid id)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with id {id} not found");

        return reservation;
    }

    public async Task<Reservation> DeleteByIdAsync(Guid id)
    {
        var reservation = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(reservation);
    }

    public async Task<List<Reservation>> GetAllByDateReservedSince(DateTime date)
    {
        var reservation = await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.ReservedAt < date && x.Status == ReservationStatus.Pending
        );
        return reservation.ToList();
    }

    public async Task<Reservation> ExpireAsync(Reservation reservation)
    {
        reservation.Status = ReservationStatus.Expired;
        return await _repository.UpdateAsync(reservation);
    }
}