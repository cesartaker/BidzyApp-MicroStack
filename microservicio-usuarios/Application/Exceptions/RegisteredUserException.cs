using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class RegisteredUserException: Exception
{
    public RegisteredUserException(string email) : base($"Un usuario ya se ha registrado con este correo: {email}"){}
    
}
