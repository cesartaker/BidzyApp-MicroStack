using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.DTOs.MongoDTOs;

public class MongoUserDto
{
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonElement("UUID")]
    public Guid Id { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("middleName")]
    public string MiddleName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("secondLastName")]
    public string SecondLastName { get; set; }

    [BsonElement("email")]
    public Email Email { get; set; }

    [BsonElement("phoneNumber")]
    public PhoneNumber PhoneNumber { get; set; }

    [BsonElement("address")]
    public string Address { get; set; }
    [BsonRepresentation(BsonType.String)]

    [BsonElement("roleId")]
    public Guid RoleId { get; set; }

    [BsonElement("roleName")]
    public string RoleName { get; set; }

    public MongoUserDto(Guid id, string firstName, string middleName, string lastName, string secondLastName,
        Email email, PhoneNumber phoneNumber, string address, Guid roleId, string roleName)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        SecondLastName = secondLastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        RoleId = roleId;
        RoleName = roleName;
    }
}
