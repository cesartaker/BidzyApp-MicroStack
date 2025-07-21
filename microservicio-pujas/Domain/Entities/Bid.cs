using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Bid
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid AuctionId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid BidderId { get; set; }
    [BsonElement("amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }
    [BsonElement("bidderName")]
    [BsonRepresentation(BsonType.String)]
    public string BidderName { get; set; } = string.Empty;
    [BsonElement("CreatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Bid(){}

    public Bid(Guid auctionId, Guid bidderId, decimal amount,string bidderName)
    {
        Id = Guid.NewGuid();
        AuctionId = auctionId;
        BidderId = bidderId;
        Amount = amount;
        BidderName = bidderName;
        CreatedAt = DateTime.UtcNow;
    }
}
