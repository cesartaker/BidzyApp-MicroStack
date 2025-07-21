using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class NotRegisteredUserException: Exception
{
    public NotRegisteredUserException(string email):base($"No hay ningun usuario registrado con el correo: {email}"){}
}
