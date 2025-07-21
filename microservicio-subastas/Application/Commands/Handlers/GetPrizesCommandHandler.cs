using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos;
using MediatR;

namespace Application.Commands.Handlers;

public class GetPrizesCommandHandler : IRequestHandler<GetPrizesCommand, List<PrizeDto>>
{
    private readonly IPrizeService _prizeService;
    public GetPrizesCommandHandler(IPrizeService prizeService)
    {
        _prizeService = prizeService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetPrizesCommand"/> para recuperar todos los premios reclamados por el usuario.
    /// Invoca el servicio de premios para consultar los datos asociados al identificador del usuario.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetPrizesCommand"/> que contiene el identificador único del usuario cuyos premios se desean obtener.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite finalizar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{PrizeDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los objetos <see cref="PrizeDto"/> que representan los premios del usuario.
    /// </returns>
    public Task<List<PrizeDto>> Handle(GetPrizesCommand request, CancellationToken cancellationToken)
    {
        return _prizeService.GetPrizesAsync(request.userId);
    }
}
