using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonElement("subject")]
    [BsonRepresentation(BsonType.String)]
    public string Subject { get; set; }
    [BsonElement("email")]
    [BsonRepresentation(BsonType.String)]
    public string Email { get; set; }
    [BsonElement("message")]
    [BsonRepresentation(BsonType.String)]
    public string Message { get; set; }
    [BsonElement("submittedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SubmittedAt { get; set; }

    public Notification(){}

    public Notification(string email, string message, string subject)
    {
        Id = Guid.NewGuid();
        Email = email;
        Message = message;
        SubmittedAt = DateTime.UtcNow;
        Subject = subject;
    }
}
