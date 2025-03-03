using Microsoft.EntityFrameworkCore;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Models.Data;
using ProductProvider.Models.Data.Entities;
using System;
using System.Threading.Tasks;

namespace ProductProvider.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ProductDbContext _context;

    public ReservationRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ReservationEntity reservation)
    {
        await _context.Set<ReservationEntity>().AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<ReservationEntity> GetReservationByUserIdAsync(Guid companyId)
    {
        return await _context.Set<ReservationEntity>()
                             .FirstOrDefaultAsync(r => r.UserId == companyId);
    }

    public async Task DeleteAsync(Guid reservationId)
    {
        var reservation = await _context.Set<ReservationEntity>().FindAsync(reservationId);
        if (reservation != null)
        {
            _context.Set<ReservationEntity>().Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
