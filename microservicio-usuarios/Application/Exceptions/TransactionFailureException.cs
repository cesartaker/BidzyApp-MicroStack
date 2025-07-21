using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class TransactionFailureException: Exception
{
    public TransactionFailureException(string message): base(message) { }
}
