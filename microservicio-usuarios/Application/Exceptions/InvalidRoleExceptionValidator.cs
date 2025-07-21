using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class InvalidRoleExceptionValidator : Exception
    {
        public InvalidRoleExceptionValidator(): base ("Los campos no pueden ser nulos"){ }
    }
}
