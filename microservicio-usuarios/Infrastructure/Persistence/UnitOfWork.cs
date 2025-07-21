using System.Net;
using Application.Contracts;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using RestSharp;


namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task<HttpStatusCode> CommitAsync()
    {
        try
        {
            var sqlResult = await _context.SaveChangesAsync();

            if (sqlResult > 0 && _context.Database.CurrentTransaction != null)
            {
                await _context.Database.CommitTransactionAsync();
                return HttpStatusCode.OK; // Indica éxito
            }

            await _context.Database.RollbackTransactionAsync();
            throw new Exception("No se realizaron cambios en la base de datos");
        }
        catch (Exception ex)
        {
            if (_context.Database.CurrentTransaction != null) // Verifica la transacción antes de hacer rollback
            {
                await _context.Database.RollbackTransactionAsync();
            }

            Console.WriteLine($"Error inesperado: {ex.Message}");
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}
