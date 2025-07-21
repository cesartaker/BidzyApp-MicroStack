using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions;

public class UpdateProductException : Exception
{
    public UpdateProductException() : base("Error al actualizar el producto") { }
    public UpdateProductException(string message) : base(message) { }
    public UpdateProductException(string message, Exception innerException) : base(message, innerException) { }
}
