using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class CreatedUserResponseDto
{
    public string FullName {  get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    public string Message { get; set; }

    public CreatedUserResponseDto(string name, string middleName, string lastName, string secondLastName, 
        string email, string roleName,string message)
    {
        FullName = $"{name} {middleName} {lastName} {secondLastName}";
        Email = email ;
        RoleName = roleName ;
        Message = message;
    }
}
