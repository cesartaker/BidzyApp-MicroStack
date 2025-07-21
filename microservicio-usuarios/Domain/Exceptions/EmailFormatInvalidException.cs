using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects;

namespace Domain.Exceptions;

public class EmailFormatInvalidException: Exception
{
    public EmailFormatInvalidException(string email)
        : base($"El correo '{email}' debe terminar en @gmail.com")
    {
    }
}
  

