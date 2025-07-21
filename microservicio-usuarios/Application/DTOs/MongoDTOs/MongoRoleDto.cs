using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.DTOs.MongoDTOs;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class MongoRoleDto
{
    [BsonId] // Especifica que MongoDB mapeará `_id` a `Id`
    [BsonRepresentation(BsonType.ObjectId)] // Convierte ObjectId a string al serializar
    public string Id { get; set; }

    [BsonElement("ID")] // Mapea el UUID de PostgreSQL
    public string PostgresID { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }

    public MongoRoleDto(string id, string postgresId, string name)
    {
        Id = id; // `_id` de MongoDB
        PostgresID = postgresId; // ID de PostgreSQL
        Name = name;
    }
}
