using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.MongoDTOs;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<MongoUserDto> Users => _database.GetCollection<MongoUserDto>("Users");
    public IMongoCollection<MongoRoleDto> Rols => _database.GetCollection<MongoRoleDto>("Roles");
    
    public MongoDbContext(string connectionString, string databaseName)
    {
        _database = new MongoClient(connectionString).GetDatabase(databaseName);
    }

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

}
