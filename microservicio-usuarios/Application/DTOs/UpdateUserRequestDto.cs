using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.DTOs;

public record UpdateUserRequest(string? FirstName, string? MiddleName, string? LastName, string? SecondLastName,
    string? PhoneNumber, string? Address);
