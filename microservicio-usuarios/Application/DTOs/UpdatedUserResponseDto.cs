using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class UpdatedUserResponseDto
{
    public string FirstName { get; set; }
    public string MiddleName {  get; set; }
    public string Lastname { get; set; }
    public string SecondLastName { get; set; }
    public string PhoneNumber {  get; set; }
    public string Address { get; set; }

    public UpdatedUserResponseDto(string firstName, string middleName, string lastName, string secondLastName, string phoneNumber, string address)
    {
        FirstName = firstName;
        MiddleName = middleName;
        Lastname = lastName;
        SecondLastName = secondLastName;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}
