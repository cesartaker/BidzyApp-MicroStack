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
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");

    public MongoDbReadContext(string connectionString, string databaseName)
    {
        _database = new MongoClient(connectionString).GetDatabase(databaseName);
    }

    public MongoDbReadContext(IMongoDatabase database)
    {
        _database = database;
    }
}
