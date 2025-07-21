using System.Text.Json.Serialization;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Auction
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    public AuctionStatus Status { get; set; }
    [BsonElement("claimed")]
    public bool claimed { get; set; } = false;
    [BsonElement("recepted")]
    public bool recepted { get; set; } = false;

    public Auction() { }

    public Auction(Guid userId, Guid productId, string name, string description, decimal basePrice, decimal reservePrice,
         DateTime endDate, decimal minBidStep, string imageUrl)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        Name = name;
        Description = description;
        BasePrice = basePrice;
        ReservePrice = reservePrice;
        StartDate = DateTime.Now;
        EndDate = endDate;
        MinBidStep = minBidStep;
        ImageUrl = imageUrl;
        Status = AuctionStatus.Active;
    }

    public void SetWinner(Guid winnerId)
    {
        WinnerId = winnerId;
    }

    public void SetState (AuctionStatus status)
    {
        Status = status;
    }

    public void SetClaimed(bool claimed)
    {
        this.claimed = claimed;
    }
    public void SetRecepted(bool recepted)
    {
        this.recepted = recepted;
    }

}
