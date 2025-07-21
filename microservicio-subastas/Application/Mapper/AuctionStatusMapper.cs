using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Mapper;

public static class AuctionStatusMapper
{
    /// <summary>
    /// Convierte una lista de cadenas en una lista de valores del tipo <see cref="AuctionStatus"/>, ignorando diferencias de mayúsculas.
    /// Los valores inválidos o no reconocidos se descartan silenciosamente.
    /// </summary>
    /// <param name="values">
    /// Lista de <see cref="string"/> que representan los nombres de estados de subasta a convertir.
    /// </param>
    /// <returns>
    /// Una lista de <see cref="AuctionStatus"/> que contiene únicamente los valores válidos convertidos a partir de las cadenas proporcionadas.
    /// </returns>
    public static List<AuctionStatus> MapFromStrings(List<string> values)
    {
        return values
            .Select(v => Enum.TryParse<AuctionStatus>(v, true, out var status) ? status : (AuctionStatus?)null)
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
    }

}
