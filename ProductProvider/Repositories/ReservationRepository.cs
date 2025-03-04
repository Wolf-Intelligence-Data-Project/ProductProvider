using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Models.Data;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ProductDbContext _context;
    private readonly string _connectionString;

    public ReservationRepository(ProductDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("ProductDatabase");
    }


    #region Products Table 
    // This part communicates with products table
    public async Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId)
    {
        var sql = @"
            UPDATE Products
            SET ReservedBy = @CompanyId, ReservedUntil = @ReservedUntil
            WHERE ProductId IN (SELECT value FROM STRING_SPLIT(@ProductIds, ','))";

        var parameters = new DynamicParameters();
        parameters.Add("CompanyId", companyId);
        parameters.Add("ReservedUntil", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")).AddMinutes(15));
        parameters.Add("ProductIds", string.Join(",", productIds));

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task DeleteExpiredReservationsAsync(DateTime cutoffTime)
    {
        // Start a transaction to ensure atomicity
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Execute SQL to update expired product reservations
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            UPDATE Products
            SET ReservedUntil = NULL, ReservedBy = NULL
            WHERE ReservedUntil < @CutoffTime";

            await connection.ExecuteAsync(sql, new { CutoffTime = cutoffTime });

            // Use Entity Framework to find expired reservations (based on ReservedTime)
            var expiredReservations = await _context.Set<ReservationEntity>()
                                                     .Where(r => r.ReservedTime < cutoffTime)
                                                     .ToListAsync();

            // Remove expired reservations
            if (expiredReservations.Any())
            {
                _context.Set<ReservationEntity>().RemoveRange(expiredReservations);
            }

            // Commit the transaction to save changes to both the Products and ReservationEntity tables
            await _context.SaveChangesAsync();
            await transaction.CommitAsync(); // Commit the transaction

        }
        catch (Exception ex)
        {
            // Rollback the transaction in case of an error
            await transaction.RollbackAsync();
            // Log the error (optional)
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }


    public async Task UpdateReservationsAsync(Guid companyId)
    {
        using var connection = new SqlConnection(_connectionString);

        string sql = @"
        UPDATE Products
        SET ReservedUntil = NULL, ReservedBy = NULL
        WHERE Reservedby = @UserId";

        await connection.ExecuteAsync(sql, new { UserId = companyId });


    }
    #endregion

    #region Reservations Table
    // This part communicates with products table
    public async Task AddReservationAsync(ReservationEntity reservation)
    {
        await _context.Set<ReservationEntity>().AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<ReservationEntity> GetReservationByUserIdAsync(Guid companyId)
    {
        return await _context.Set<ReservationEntity>()
                             .FirstOrDefaultAsync(r => r.UserId == companyId);
    }
    public async Task DeleteReservationImmediatelyAsync(Guid reservationId)
    {
        var reservation = await _context.Set<ReservationEntity>().FindAsync(reservationId);
        if (reservation != null)
        {
            _context.Set<ReservationEntity>().Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }

    #endregion
}
