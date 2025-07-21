
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Application.Commands;
using Application.Contracts;
using Application.Dtos;
using Application.Exceptions;
using Domain.Contracts.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class AutoBidsService : IAutoBidsService
{
    private readonly ConcurrentDictionary<Guid, List<AutoBid>> _autoBids;
    private readonly IBidsSocketHub _hub;
    private readonly ICacheAuctionsStates _cacheAuctionsStates;
    private readonly IServiceScopeFactory _scopeFactory;

    public AutoBidsService(IServiceScopeFactory scopeFactory, IBidsSocketHub hub, ICacheAuctionsStates cacheAuctionsStates)
    {
        _autoBids = new ConcurrentDictionary<Guid, List<AutoBid>>();
        _scopeFactory = scopeFactory;
        _hub = hub;
        _cacheAuctionsStates = cacheAuctionsStates;
    }
    /// <summary>
    /// Ejecuta el proceso automatizado de pujas para una subasta específica.
    /// Crea un scope de servicio para obtener dependencias necesarias, valida el estado activo de la subasta,
    /// calcula el monto mínimo requerido en función de la puja más alta o el precio base,
    /// y genera pujas automáticas dentro del rango definido por cada configuración.
    /// Publica cada puja exitosa en el canal correspondiente y limpia datos obsoletos al final.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) para la cual se deben ejecutar las pujas automáticas.
    /// </param>
    public async Task ExecuteAutoBids(Guid auctionId)
    {
        using var scope = _scopeFactory.CreateScope();
        var bidsService = scope.ServiceProvider.GetRequiredService<IBidsService>();
        var auctionsService = scope.ServiceProvider.GetRequiredService<IAuctionsService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();


        if (!_autoBids.TryGetValue(auctionId, out var autoBidList) || autoBidList.Count == 0)
            return;
        decimal currentAmount = 0;
        var auction = await auctionsService.GetAuction(auctionId);
        var candidates = autoBidList.ToList(); 

        foreach (var config in candidates)
        {
            if (!_cacheAuctionsStates.IsActive(auctionId.ToString()))
            {
                ClearAutoBids(auctionId);
                break;
            }

            var highestBid = await bidsService.GetHighestBidByAuctionIdAsync(auctionId);
            if (highestBid == null)
            {
                currentAmount = auction!.BasePrice;
            }
            else
            {
                currentAmount = highestBid.Amount;
            }

            var nextAmount = currentAmount + config.MinBidIncrease;
            
            if (nextAmount > config.MaxAmount)
            {
                RemoveAutoBid(auctionId, config.BidderId); 
                continue;
            }

            var command = new NewBidCommand
            {
                AuctionId = auctionId,
                BidderId = config.BidderId,
                bidderName = config.BidderName,
                Amount = nextAmount
            };
            
            var bidResult = await mediator.Send(command);

            if (bidResult != null)
            {
                await _hub.BroadcastAutoBidAsync(auctionId.ToString(), bidResult);
            }

            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        
        var lastBid = await bidsService.GetHighestBidByAuctionIdAsync(auctionId);
        CleanObsolete(auctionId, lastBid.Amount);

    }
    /// <summary>
    /// Registra una configuración de puja automática (<see cref="AutoBid"/>) para una subasta específica.
    /// Valida que el incremento mínimo cumpla con el paso de puja requerido por la subasta.
    /// Si la configuración es válida, la agrega o actualiza en el diccionario de pujas automáticas.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) sobre la cual se configura la puja automática.
    /// </param>
    /// <param name="bidderId">
    /// Identificador único del usuario (<see cref="Guid"/>) que desea participar con pujas automáticas.
    /// </param>
    /// <param name="bidderName">
    /// Nombre del postor que será utilizado como referencia en las pujas.
    /// </param>
    /// <param name="maxAmount">
    /// Monto máximo permitido para pujar de forma automática (<see cref="decimal"/>).
    /// </param>
    /// <param name="minBidIncrease">
    /// Incremento mínimo por puja que debe cumplir con el paso mínimo exigido por la subasta.
    /// </param>
    public async Task RegisterAutoBid(Guid auctionId, Guid bidderId, string bidderName, decimal maxAmount, decimal minBidIncrease)
    {
        using var scope = _scopeFactory.CreateScope();
        var auctionsService = scope.ServiceProvider.GetRequiredService<IAuctionsService>();
        var auction = await auctionsService.GetAuction(auctionId);
        var autoBid = new AutoBid(bidderId, bidderName, maxAmount, minBidIncrease);

        if (minBidIncrease < auction.MinBidStep)
            throw new MinBidIncreaseException();

        _autoBids.AddOrUpdate(auctionId,new List<AutoBid> { autoBid },
            (key, existingList) =>
            {
                existingList.RemoveAll(b => b.BidderId == bidderId);
                existingList.Add(autoBid);
                return existingList;
            }
        );
    }
    /// <summary>
    /// Elimina la configuración de puja automática de un postor específico en una subasta determinada.
    /// Accede al diccionario de pujas automáticas y remueve todas las entradas que coincidan con el <paramref name="bidderId"/> dentro de la subasta indicada.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) en la que se desea eliminar la configuración de pujas automáticas.
    /// </param>
    /// <param name="bidderId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuya configuración automática debe ser removida.
    /// </param>
    private void RemoveAutoBid(Guid auctionId, Guid bidderId)
    {
        if (_autoBids.TryGetValue(auctionId, out var bidList))
        {
            bidList.RemoveAll(b => b.BidderId == bidderId);
        }
    }
    /// <summary>
    /// Elimina todas las configuraciones de puja automática asociadas a una subasta específica.
    /// Utiliza <c>TryRemove</c> sobre el diccionario de pujas automáticas para borrar el registro correspondiente al <paramref name="auctionId"/>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único (<see cref="Guid"/>) de la subasta cuya configuración de pujas automáticas debe ser eliminada.
    /// </param>
    private void ClearAutoBids(Guid auctionId)
    {
        _autoBids.TryRemove(auctionId, out _);
    }
    /// <summary>
    /// Elimina las configuraciones de puja automática que ya no pueden superar el monto actual de la subasta.
    /// Evalúa cada configuración en función del monto más reciente y el incremento mínimo requerido.
    /// Si el siguiente monto excede el máximo permitido por el usuario, la configuración se elimina.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) cuya configuración se desea depurar.
    /// </param>
    /// <param name="lastAmount">
    /// Monto actual más alto registrado en la subasta (<see cref="decimal"/>), utilizado como base para calcular la próxima puja.
    /// </param>
    private void CleanObsolete(Guid auctionId, decimal lastAmount)
    {
        if (_autoBids.TryGetValue(auctionId, out var bids))
        {
            bids.RemoveAll(b => lastAmount + b.MinBidIncrease > b.MaxAmount);
        }
    }
}
