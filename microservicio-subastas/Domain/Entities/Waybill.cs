
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Waybill
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid AuctionId { get; set; }
    [BsonElement("receptorName")]
    public string ReceptorName { get; set; }
    [BsonElement("address")]
    public string Address { get; set; }
    [BsonElement("deliveryMethod")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    public DeliveryMethod DeliveryMethod { get; set; }

    public Waybill(){}

    public Waybill(Guid auctionId,string receptorName, string address, DeliveryMethod deliveryMethod)
    {
        Id = Guid.NewGuid();
        AuctionId = auctionId;
        ReceptorName = receptorName;
        Address = address;
        DeliveryMethod = deliveryMethod;
    }
}
