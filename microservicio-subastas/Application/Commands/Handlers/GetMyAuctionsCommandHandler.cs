using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using MediatR;

namespace Application.Commands.Handlers;

public class GetMyAuctionsCommandHandler : IRequestHandler<GetMyAuctionsCommand, AuctionsDto>
{
    private readonly IAuctionService _auctionService;

    public GetMyAuctionsCommandHandler(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetMyAuctionsCommand"/> para recuperar todas las subastas creadas por un usuario específico.
    /// Realiza una consulta al servicio de subastas utilizando el identificador del usuario y encapsula el resultado en un objeto <see cref="AuctionsDto"/>.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetMyAuctionsCommand"/> que contiene el identificador único del usuario cuyas subastas se desean consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite abortar la operación asincrónica de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{AuctionsDto}"/> que representa la ejecución asincrónica del comando,
    /// devolviendo la colección de subastas pertenecientes al usuario solicitado.
    /// </returns>
    public async Task<AuctionsDto> Handle(GetMyAuctionsCommand request, CancellationToken cancellationToken)
    {
        return new AuctionsDto(await _auctionService.GetMyAuctionsAsync(request.userId));
    }
}
