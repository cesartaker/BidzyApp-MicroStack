
using System.Text.Json.Serialization;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Complaint
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    [BsonElement("reason")]
    public string Reason { get; set; }
    [BsonElement("description")]
    public string Description { get; set; }
    [BsonElement("evidenceUrl")]
    public string EvidenceUrl { get; set; }
    [BsonElement("solution")]
    public string Solution { get; set; }
    [BsonRepresentation(BsonType.String)]

    public DateTime CreatedAt { get; set; }
    [BsonRepresentation(BsonType.String)]

    public DateTime ResolveAt { get; set; }
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ComplaintStatus Status { get; set; }
    public Complaint() { }
    public Complaint(Guid userId,string reason, string description, string evidenceUrl,ComplaintStatus status)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Reason = reason;
        Description = description;
        EvidenceUrl = evidenceUrl;
        CreatedAt = DateTime.UtcNow;
        Status = status;
    }

    public void Resolve(string solution)
    {
        Solution = solution;
        Status = ComplaintStatus.Resolved;
        ResolveAt = DateTime.UtcNow;
    }
}
