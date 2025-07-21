using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;



public class Payment
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]

    public Guid UserId { get; set; }

    [BsonElement("auctionId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? AuctionId { get; set; }         

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; }

    [BsonElement("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    public PaymentStatus Status { get; set; }             

    [BsonElement("paymentIntentId")]
    public string? PaymentIntentId { get; set; }   

    [BsonElement("paymentMethodId")]
    public string? PaymentMethodId { get; set; }   

    [BsonElement("paidAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? PaidAt { get; set; }          

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public Payment(){}

    public Payment(Guid userId,Guid auctionId, decimal amount)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Currency = "usd";
        AuctionId = auctionId;
        Amount = amount;
        Status = PaymentStatus.pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePayment(string paymentIntentId,string paymentMethodId)
    {
        PaymentIntentId = paymentIntentId;
        PaymentMethodId = paymentMethodId;
        CreatedAt = DateTime.UtcNow;
        Status = PaymentStatus.paid;
        PaidAt = DateTime.UtcNow;
    }
}
