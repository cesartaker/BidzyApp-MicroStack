using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class InvalidBidException: Exception
{
    public InvalidBidException() : base("Puja Invalida: El monto debe cumplir con el incremento mínimo y ser superior a la puja actual más alta") { }

}
