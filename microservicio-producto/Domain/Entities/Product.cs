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

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid AuctioneerId { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("description")]

    public string Description { get; set; }
    [BsonElement("category")]

    public string Category { get; set; }
    [BsonElement("basePrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal BasePrice {  get; set; }
    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; }
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductStatus Status { get; set; }

    public Product(){}

    public Product(Guid auctioneerId, string name, string description, decimal basePrice, string category, string imageUrl,ProductStatus status)
    {
        Id = Guid.NewGuid();
        AuctioneerId = auctioneerId;
        Name = name;
        Description = description;
        BasePrice = basePrice;
        Category = category;
        ImageUrl = imageUrl;
        Status = status;
    }
    [JsonConstructor]
    [BsonConstructor]
    public Product(Guid id,Guid auctioneerId, string name, string description, decimal basePrice, string category, string imageUrl, ProductStatus status)
    {
        Id = id;
        AuctioneerId = auctioneerId;
        Name = name;
        Description = description;
        BasePrice = basePrice;
        Category = category;
        ImageUrl = imageUrl;
        Status = status;
    }
}
