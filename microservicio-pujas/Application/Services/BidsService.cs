using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Repositories;
using Domain.Contracts.Services;
using Domain.Entities;

namespace Application.Services;

public class BidsService : IBidsService
{
    private readonly IMongoBidsReadRepository _bidsReadRepository;
    private readonly IMongoBidsWriteRepository _bidsWriteRepository;

    public BidsService(IMongoBidsReadRepository read,IMongoBidsWriteRepository write)
    {
        _bidsReadRepository = read;
        _bidsWriteRepository = write;
    }
    /// <summary>
    /// Agrega una nueva puja (<see cref="Bid"/>) a una subasta, validando que el monto ofertado cumpla con el mínimo requerido.
    /// Recupera la puja más alta actual, calcula el mínimo aceptable en función del precio base y el incremento mínimo,
    /// y si el monto es válido, registra la puja en el repositorio de escritura.
    /// </summary>
    /// <param name="bid">
    /// Objeto <see cref="Bid"/> que contiene los datos de la puja a registrar (monto, postor, subasta asociada).
    /// </param>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que representa la subasta actual sobre la cual se realiza la puja.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna <c>BadRequest</c> si el monto ofertado no cumple con el mínimo requerido;
    /// en caso contrario, delega el guardado al repositorio y retorna el código de estado correspondiente.
    /// </returns>
    public async Task<HttpStatusCode> AddBidAsync(Bid bid, Auction auction)
    {
        var highestBid = await _bidsReadRepository.GetHighestBidByAuctionIdAsync(auction.Id);

        var minimumRequired = highestBid?.Amount ?? auction.BasePrice;
        var requiredBid = minimumRequired + auction.MinBidStep;

        if (bid.Amount < requiredBid)
            return HttpStatusCode.BadRequest;

        return await _bidsWriteRepository.AddBidAsync(bid);
    }
    /// <summary>
    /// Recupera todas las pujas (<see cref="Bid"/>) asociadas a una subasta específica mediante su identificador.
    /// Invoca el repositorio de lectura y retorna los datos obtenidos, o una lista vacía si no existen pujas registradas.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica:
    /// retorna una lista de objetos <see cref="Bid"/> si existen registros asociados;
    /// si no se encuentran pujas, retorna una lista vacía para asegurar consistencia en el consumo.
    /// </returns>
    public async Task<List<Bid>> GetBidsByAuctionIdAsync(Guid auctionId)
    {
        var bids = await _bidsReadRepository.GetBidsByAuctionId(auctionId);
        return bids ?? new List<Bid>();
    }
    /// <summary>
    /// Recupera todas las pujas (<see cref="Bid"/>) asociadas a un conjunto de subastas, identificadas por sus respectivos <see cref="Guid"/>.
    /// Delega la lógica directamente al repositorio de lectura <see cref="_bidsReadRepository"/>.
    /// </summary>
    /// <param name="auctionIds">
    /// Lista de identificadores únicos (<see cref="Guid"/>) de las subastas que se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica:
    /// retorna una lista de objetos <see cref="Bid"/> correspondientes a las subastas indicadas.
    /// </returns>
    public Task<List<Bid>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds)
    {
        return _bidsReadRepository.GetBidsByAuctionIdsAsync(auctionIds);
    }
    /// <summary>
    /// Recupera la puja más alta (<see cref="Bid"/>) asociada a una subasta específica utilizando su identificador único.
    /// Delega la operación al repositorio de lectura <see cref="_bidsReadRepository"/> sin aplicar lógica adicional.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) cuya puja más alta se desea consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Bid}"/> que representa la operación asincrónica:
    /// devuelve el objeto <see cref="Bid"/> correspondiente a la oferta más alta registrada en la subasta indicada.
    /// </returns>
    public async Task<Bid> GetHighestBidByAuctionIdAsync(Guid auctionId)
    {
        return await _bidsReadRepository.GetHighestBidByAuctionIdAsync(auctionId);
    }
}
