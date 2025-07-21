using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Mappers;

public static class ProductStatusMapper
{
    /// <summary>
    /// Convierte una cadena de texto en su correspondiente valor del enumerado <see cref="ProductStatus"/>.
    /// El método no distingue mayúsculas de minúsculas.
    /// Lanza una excepción si el valor no corresponde a ningún estado válido.
    /// </summary>
    /// <param name="value">
    /// Cadena que representa un valor posible del enumerado <see cref="ProductStatus"/>.
    /// </param>
    /// <returns>
    /// Valor <see cref="ProductStatus"/> correspondiente a la cadena proporcionada.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si la cadena no puede ser convertida en un valor válido de <see cref="ProductStatus"/>.
    /// </exception>
    public static ProductStatus MapFromString(string value)
    {
        return Enum.TryParse<ProductStatus>(value, true, out var status)
        ? status : throw new ArgumentException($"'{value}' no es un valor válido de ProductStatus");
;
    }
}
