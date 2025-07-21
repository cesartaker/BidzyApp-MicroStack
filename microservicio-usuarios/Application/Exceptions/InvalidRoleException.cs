using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class InvalidRoleException: Exception
{
    public InvalidRoleException() : base("No se encontró el rol especificado") { }
        
    
}
