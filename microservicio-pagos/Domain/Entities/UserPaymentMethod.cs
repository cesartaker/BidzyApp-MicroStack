using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities;

public class UserPaymentMethod
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    public string PaymentMethodId { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public string ExpMonth { get; set; } = string.Empty;
    public string ExpYear { get; set; } = string.Empty;

    public UserPaymentMethod() { }
    public UserPaymentMethod(Guid userId, string paymentMethodId, string brand,string last4,
        string expMonth,string expYear)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        PaymentMethodId = paymentMethodId;
        Brand = brand;
        Last4 = last4;
        ExpMonth = expMonth;
        ExpYear = expYear;
    }
}
