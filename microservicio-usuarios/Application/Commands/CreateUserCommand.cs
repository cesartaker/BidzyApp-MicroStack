using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Commands;



public record CreateUserCommand(string FirstName, string MiddleName, string LastName, string SecondLastName, 
    string Email, string PhoneNumber, string Address, string rolName,string password) : IRequest<CreatedUserResponseDto>;
