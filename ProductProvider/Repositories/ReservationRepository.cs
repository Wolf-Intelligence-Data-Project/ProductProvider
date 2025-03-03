using Dapper;
using Microsoft.EntityFrameworkCore;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Models.Data;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ProductDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationRepository(ProductDbContext context, IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _scopeFactory = scopeFactory;
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

    public async Task DeleteAsync(Guid companyId)
    {
        var reservation = await _context.Set<ReservationEntity>().FindAsync(companyId);
        if (reservation != null)
        {
            _context.Set<ReservationEntity>().Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReleaseExpiredReservationsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        var connection = context.Database.GetDbConnection(); // Use GetDbConnection()

        var stockholmTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                              TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

        string sql = @"
                UPDATE Products
                SET ReservedUntil = NULL, ReservedBy = NULL
                WHERE ReservedUntil < @StockholmTime";

        await connection.ExecuteAsync(sql, new { StockholmTime = stockholmTime });
    }
}
