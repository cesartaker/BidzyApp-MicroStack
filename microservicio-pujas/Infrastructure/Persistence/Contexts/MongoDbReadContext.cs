using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Contexts;

public class MongoDbReadContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<Bid> Bids => _database.GetCollection<Bid>("Bids");

    public MongoDbReadContext(string connectionString, string databaseName)
    {
        _database = new MongoClient(connectionString).GetDatabase(databaseName);
    }

    public MongoDbReadContext(IMongoDatabase database)
    {
        _database = database;
    }
}
