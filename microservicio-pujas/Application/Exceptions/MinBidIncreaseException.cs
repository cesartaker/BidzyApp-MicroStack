using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class MinBidIncreaseException : Exception
{
    public MinBidIncreaseException() : base("El monto mínimo de incremento indicado es menor al incremento minimo de la subasta por puja") {}
}

