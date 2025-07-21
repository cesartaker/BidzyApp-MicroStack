using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Commands;

public record UpdateUserCommand(string UserId,string? FirstName, string? MiddleName, string? LastName, string? SecondLastName, 
    string? PhoneNumber, string? Address, string Email) : IRequest<UpdatedUserResponseDto>;


