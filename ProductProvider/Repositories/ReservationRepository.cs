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
            SET CustomerId = @CompanyId, ReservedUntil = @ReservedUntil
            WHERE ProductId IN (SELECT value FROM STRING_SPLIT(@ProductIds, ','))";
        
        var parameters = new DynamicParameters();
        parameters.Add("CompanyId", companyId);
        parameters.Add("ReservedUntil", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Time")).AddMinutes(15));
        parameters.Add("ProductIds", string.Join(",", productIds));

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task DeleteExpiredReservationsAsync(ProductDbContext context, DateTime cutoffTime, Guid companyId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Update expired products
            string updateProductsSql = @"
            UPDATE Products
            SET ReservedUntil = NULL, CustomerId = NULL
            WHERE ReservedUntil < @CutoffTime"; 

            int rowsAffected = await connection.ExecuteAsync(updateProductsSql, new { CutoffTime = cutoffTime }, transaction);

            // Check if any rows were updated
            if (rowsAffected == 0)
            {
                Console.WriteLine("No expired products were updated.");
            }

            // Delete expired reservations for the given company
            string deleteReservationsSql = @"
            DELETE FROM Reservations
            WHERE CustomerId = @UserId"; 

            int deletedRows = await connection.ExecuteAsync(deleteReservationsSql, new { UserId = companyId }, transaction);

            // Optionally, check if any reservations were deleted
            if (deletedRows == 0)
            {
                Console.WriteLine("No expired reservations were deleted.");
            }

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            // Rollback the transaction in case of an error
            await transaction.RollbackAsync();
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateReservationsAsync(Guid companyId)
    {
        using var connection = new SqlConnection(_connectionString);

        string sql = @"
        UPDATE Products
        SET ReservedUntil = NULL, CustomerId = NULL
        WHERE CustomerId = @UserId";

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
                             .FirstOrDefaultAsync(r => r.CustomerId == companyId);
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
