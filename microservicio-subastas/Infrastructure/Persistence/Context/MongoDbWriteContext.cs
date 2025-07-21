using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Context;

public class MongoDbWriteContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<Auction> Auctions => _database.GetCollection<Auction>("Auctions");
    public IMongoCollection<Waybill> Waybills => _database.GetCollection<Waybill>("Waybills");

    public MongoDbWriteContext(string connectionString, string databaseName)
    {
        _database = new MongoClient(connectionString).GetDatabase(databaseName);
    }

    public MongoDbWriteContext(IMongoDatabase database)
    {
        _database = database;
    }
}
