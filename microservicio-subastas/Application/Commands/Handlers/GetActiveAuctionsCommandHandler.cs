using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Handlers;

public class GetActiveAuctionsCommandHandler : IRequestHandler<GetActiveAuctionsCommand, AuctionsDto>
{
    private readonly IAuctionService _auctionService;

    public GetActiveAuctionsCommandHandler(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetActiveAuctionsCommand"/> para recuperar todas las subastas activas en el sistema.
    /// Realiza una consulta al servicio de subastas utilizando el estado <see cref="AuctionStatus.Active"/> y encapsula el resultado en un <see cref="AuctionsDto"/>.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetActiveAuctionsCommand"/> que inicia la solicitud para obtener las subastas activas.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que permite cancelar la operación asincrónica de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{AuctionsDto}"/> que representa la operación asincrónica y devuelve
    /// un objeto <see cref="AuctionsDto"/> con la colección de subastas activas.
    /// </returns>
    public async Task<AuctionsDto> Handle(GetActiveAuctionsCommand request, CancellationToken cancellationToken)
    {
        return new AuctionsDto(await _auctionService.GetAuctionsAsync(AuctionStatus.Active));
    }
}
