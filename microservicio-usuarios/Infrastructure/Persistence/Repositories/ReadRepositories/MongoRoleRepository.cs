using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Infrastructure.Persistence.Context;
using Domain.ValueObjects;
using Application.DTOs.MongoDTOs;
using System.Data;
using Application.Contracts.Repositories;

namespace Infrastructure.Persistence.Repositories.ReadRepositories;

public class MongoRoleRepository : IMongoRoleRepository
{
    private readonly AppDbContext _context;
    private readonly MongoDbContext _mongoContext;

    public MongoRoleRepository(AppDbContext context, MongoDbContext mongoContext)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
    }

    public async Task<string?> IdRoleByName(string roleName)
    {
        try
        {

            var filter = Builders<MongoRoleDto>.Filter.Eq(p => p.Name, roleName);
            var _roleName = await _mongoContext.Rols.Find(filter).FirstOrDefaultAsync();
            if (_roleName != null)
            {
                return _roleName.PostgresID;
            }
            else
            {
                throw new Exception($"El permiso no esta disponible para el rol o no existe");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener el rol por nombre: {ex.Message}", ex);
        }
    }

}