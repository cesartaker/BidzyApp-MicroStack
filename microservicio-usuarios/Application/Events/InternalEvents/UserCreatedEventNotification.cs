using Domain.Enums;
using Domain.ValueObjects;
using MediatR;


namespace Application.Events.InternalEvents;

public class UserCreatedEventNotification : INotification
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string SecondLastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public Guid RoleId { get; private set; }
    public string RoleName { get; private set; }
    public UserCreatedEventNotification(Guid userId, string firstName, string middleName, string lastName,
        string secondLastName, Email email, PhoneNumber phoneNumber, string address, Guid roleId, string roleName)
    {
        UserId = userId;
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
