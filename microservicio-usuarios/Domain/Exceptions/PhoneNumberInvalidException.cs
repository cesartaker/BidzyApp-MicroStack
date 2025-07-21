using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    internal class PhoneNumberInvalidException: Exception
    {
        public PhoneNumberInvalidException(string phoneNumber)
            : base($"El número de teléfono '{phoneNumber}' no es valido para venezuela")
        {    
        }
    }
}
