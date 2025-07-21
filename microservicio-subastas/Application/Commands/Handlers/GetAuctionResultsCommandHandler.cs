using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Builders;
using Application.Dtos.AuctionResults;
using Application.Mapper;
using MediatR;

namespace Application.Commands.Handlers;

public class GetAuctionResultsCommandHandler : IRequestHandler<GetAuctionResultsCommand, List<AuctionResultDto>>
{
    private readonly IAuctionBuilder _auctionBuilder;

    public GetAuctionResultsCommandHandler(IAuctionBuilder auctionBuilder)
    {
        _auctionBuilder = auctionBuilder;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetAuctionResultsCommand"/> para recuperar el historial de subastas filtrado por estados.
    /// Convierte la lista de estados como cadenas en valores del tipo <see cref="AuctionStatus"/> y
    /// obtiene los resultados de subastas para el usuario especificado.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="GetAuctionResultsCommand"/> que contiene el identificador del usuario y la lista de estados en formato texto.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación asincrónica si se requiere.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionResultDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los resultados de subastas según los filtros especificados.
    /// </returns>
    public Task<List<AuctionResultDto>> Handle(GetAuctionResultsCommand request, CancellationToken cancellationToken)
    {
        var statuses = AuctionStatusMapper.MapFromStrings(request.statuses);
        return _auctionBuilder.GetAuctionsHistory(request.userId,statuses);
    }
}
