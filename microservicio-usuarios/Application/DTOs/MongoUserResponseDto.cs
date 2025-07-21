using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.DTOs;

public class MongoUserResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string SecondLastName { get; set; }
    public Email Email { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public string Address { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }

    public MongoUserResponseDto(Guid id, string firstName, string middleName, string lastName, string secondLastName,
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
