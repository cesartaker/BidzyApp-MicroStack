using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cache;

using System.Collections.Concurrent;
using Application.Contracts;

public class InMemoryAuctionCache : ICacheAuctionsStates
{
    private readonly ConcurrentDictionary<string, string> _activeAuctions = new();
    /// <summary>
    /// Actualiza el estado asociado a una subasta dentro del diccionario de subastas activas.
    /// Si el identificador ya existe en la colección, su estado se sobrescribe con el nuevo valor proporcionado.
    /// Si no existe, se agrega una nueva entrada.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="string"/>) que se desea actualizar en el diccionario.
    /// </param>
    /// <param name="state">
    /// Nuevo estado (<see cref="string"/>) que será asignado a la subasta.
    /// </param>
    public void Update(string auctionId, string state)
    {
        _activeAuctions[auctionId] = state;
    }
    /// <summary>
    /// Elimina el registro de una subasta activa desde el diccionario de estados.
    /// Utiliza <c>TryRemove</c> para quitar el identificador <paramref name="auctionId"/> junto con su estado asociado,
    /// si existe en la colección <c>_activeAuctions</c>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único (<see cref="string"/>) de la subasta que se desea eliminar del diccionario de estados activos.
    /// </param>
    public void Remove(string auctionId)
    {
        _activeAuctions.TryRemove(auctionId, out _);
    }
    /// <summary>
    /// Verifica si una subasta específica se encuentra actualmente marcada como activa en el sistema.
    /// Evalúa si el identificador de subasta proporcionado está presente en el diccionario <c>_activeAuctions</c>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único (<see cref="string"/>) de la subasta cuya actividad se desea validar.
    /// </param>
    /// <returns>
    /// <c>true</c> si la subasta está registrada como activa; <c>false</c> en caso contrario.
    /// </returns>
    public bool IsActive(string auctionId)
    {
        return _activeAuctions.ContainsKey(auctionId);
    }
    /// <summary>
    /// Inicializa el diccionario de subastas activas con los datos proporcionados.
    /// Elimina cualquier entrada existente y carga una nueva colección de identificadores y estados de subasta.
    /// </summary>
    /// <param name="activeAuctions">
    /// Colección de tuplas que contienen el identificador (<see cref="string"/>) y estado (<see cref="string"/>) de cada subasta a registrar como activa.
    /// </param>
    public void Initialize(IEnumerable<(string AuctionId, string State)> activeAuctions)
    {
        _activeAuctions.Clear();

        foreach (var (auctionId, state) in activeAuctions)
        {
            _activeAuctions[auctionId] = state;
        }
    }
}
