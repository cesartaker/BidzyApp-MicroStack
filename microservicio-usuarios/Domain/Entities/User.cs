using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects;
namespace Domain.Entities;

public class User
{
    public Guid Id { get;  set; }
    public string FirstName { get;  set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string SecondLastName { get; set; }
    public Email Email { get; set; }
    public  PhoneNumber? PhoneNumber { get; set; }
    public string Address { get; set; }
    public Guid RoleId { get; set; }
    
    
   
    public User() { }
    public User(string firstName, string middleName, string lastName, string secondLastName, 
        Email email, PhoneNumber phoneNumber, string address,Guid roleId)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        SecondLastName = secondLastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        RoleId = roleId;
        

    }

    public User(Guid id, string firstName, string middleName, string lastName, string secondLastName, Email email, 
        PhoneNumber phoneNumber, string address,Guid roleId)
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
    }

}

