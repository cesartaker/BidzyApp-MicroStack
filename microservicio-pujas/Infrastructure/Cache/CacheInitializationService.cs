using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Contracts.Services;

using Microsoft.Extensions.Hosting;

namespace Infrastructure.Cache;

public class CacheInitializationService : IHostedService
{
    private readonly ICacheAuctionsStates _cache;
    private readonly IAuctionsService _auctionService;

    public CacheInitializationService(ICacheAuctionsStates cacheAuctionsStates, IAuctionsService auctionsService)
    {
        _auctionService = auctionsService;
        _cache = cacheAuctionsStates;
    }
    /// <summary>
    /// Inicializa el sistema de caché con el estado de las subastas activas.
    /// Recupera la lista de subastas activas desde el servicio de subastas, extrae el identificador y estado de cada una,
    /// y alimenta el caché con estos datos para facilitar acceso rápido durante la operación del sistema.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite abortar la ejecución si así lo requiere el sistema.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de inicialización.
    /// </returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var activeAuctions = await _auctionService.GetActiveAuctions();
        var data = activeAuctions.Select(a => (a.Id.ToString(), a.Status));
        _cache.Initialize(data);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
