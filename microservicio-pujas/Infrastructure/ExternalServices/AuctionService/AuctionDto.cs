using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Infrastructure.ExternalServices.AuctionService;

public class AuctionDto
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid? WinnerId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("description")]
    public string Description { get; set; }
    [BsonElement("basePrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal BasePrice { get; set; }
    [BsonElement("reservePrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ReservePrice { get; set; }
    [BsonElement("startDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartDate { get; set; }
    [BsonElement("endDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime EndDate { get; set; }
    [BsonElement("minBidStep")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal MinBidStep { get; set; }
    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; }
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public string Status { get; set; }
}
