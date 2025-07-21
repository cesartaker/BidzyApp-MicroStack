using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AuctionService : IAuctionService
{
    IMongoAuctionWriteRepository _auctionWriteRepository;
    IMongoAuctionReadRepository _auctionReadRepository;

    public AuctionService(IMongoAuctionWriteRepository mongoAuctionWriteRepository, IMongoAuctionReadRepository mongoAuctionReadRepository)
    {
        _auctionWriteRepository = mongoAuctionWriteRepository;
        _auctionReadRepository = mongoAuctionReadRepository;
    }
    /// <summary>
    /// Cierra una subasta actualizando su estado y, si corresponde, asigna el ganador.
    /// Recupera la subasta desde el repositorio de lectura, actualiza el estado y establece el ganador si el estado es <see cref="AuctionStatus.Ended"/>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea cerrar.
    /// </param>
    /// <param name="status">
    /// Estado final que se desea asignar a la subasta (<see cref="AuctionStatus"/>), como <c>Ended</c>, <c>Deserted</c> o <c>Cancelled</c>.
    /// </param>
    /// <param name="winnerId">
    /// Identificador del usuario que ganó la subasta (<see cref="Guid"/>), utilizado únicamente si la subasta se marca como <c>Ended</c>.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Auction}"/> que representa la operación asincrónica:
    /// devuelve el objeto <see cref="Auction"/> actualizado con el nuevo estado (y ganador, si aplica).
    /// </returns>
    public Task<Auction> CloseAuctionAsync(Guid auctionId, AuctionStatus status, Guid winnerId)
    {
        var auction = _auctionReadRepository.GetAuctionById(auctionId);
        auction.Result.Status = status;
        if (status == AuctionStatus.Ended)
        {
            auction.Result.SetWinner(winnerId);
        }
        return auction;
    }
    /// <summary>
    /// Determina el estado final de una subasta en función del precio de reserva y la puja más alta recibida.
    /// Si la puja más alta es menor que el precio de reserva, la subasta se considera no adjudicada.
    /// En caso contrario, la subasta se marca como finalizada exitosamente.
    /// </summary>
    /// <param name="reservePrice">
    /// Precio de reserva (<see cref="decimal"/>): el mínimo requerido para considerar adjudicado el artículo.
    /// </param>
    /// <param name="highestBid">
    /// Monto de la puja más alta recibida (<see cref="decimal"/>) en la subasta.
    /// </param>
    /// <returns>
    /// Un valor de <see cref="AuctionStatus"/> que indica el resultado final de la subasta:
    /// <c>Unawarded</c> si no se alcanzó el precio de reserva, o <c>Ended</c> si se cumplió o superó.
    /// </returns>
    public AuctionStatus DetermineAuctionStatus(decimal reservePrice, decimal highestBid)
    {
        if (highestBid < reservePrice)
        {
            return  AuctionStatus.Unawarded;
        }
        else
        {
           return AuctionStatus.Ended; 
        }
    }
    /// <summary>
    /// Recupera una subasta específica utilizando su identificador único.
    /// Realiza una consulta al repositorio de lectura para obtener los datos completos de la subasta.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea obtener.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Auction}"/> que representa la operación asincrónica:
    /// devuelve el objeto <see cref="Auction"/> correspondiente al identificador proporcionado.
    /// </returns>
    public async Task<Auction> GetAuctionByIdAsync(Guid auctionId)
    {
        var auction = await _auctionReadRepository.GetAuctionById(auctionId);
        return auction;
    }
    /// <summary>
    /// Recupera una lista de subastas que tienen un estado específico.
    /// Realiza una consulta al repositorio de lectura utilizando el valor de <see cref="AuctionStatus"/> proporcionado.
    /// </summary>
    /// <param name="status">
    /// Estado de subasta (<see cref="AuctionStatus"/>) por el cual se filtrarán los resultados.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> cuyo estado coincide con el especificado.
    /// </returns>
    public async Task<List<Auction>> GetAuctionsAsync(AuctionStatus status)
    {
        var auctions = await _auctionReadRepository.GetAuctionsByStatus(status);
        return auctions;
    }
    /// <summary>
    /// Recupera una lista de subastas cuyos estados coinciden con alguno de los especificados en la lista proporcionada.
    /// Realiza la consulta al repositorio de lectura para filtrar las subastas por múltiples valores de <see cref="AuctionStatus"/>.
    /// </summary>
    /// <param name="statuses">
    /// Lista de valores de <see cref="AuctionStatus"/> que representan los estados por los cuales se desea filtrar las subastas.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una colección de objetos <see cref="Auction"/> que tienen alguno de los estados indicados.
    /// </returns>
    public async Task<List<Auction>> GetAuctionsByStatusesAsync(List<AuctionStatus> statuses)
    {
        return await _auctionReadRepository.GetAuctionsByStatuses(statuses);
    }
    /// <summary>
    /// Recupera las subastas creadas por un usuario específico que se encuentran en un estado determinado.
    /// Realiza una consulta al repositorio de lectura utilizando el identificador del usuario y el estado deseado.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyas subastas se desean obtener.
    /// </param>
    /// <param name="status">
    /// Estado de la subasta (<see cref="AuctionStatus"/>) por el cual se filtrarán los resultados.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los objetos <see cref="Auction"/> que corresponden al usuario y al estado indicado.
    /// </returns>
    public Task<List<Auction>> GetAuctionsByUserIdAndStatus(Guid userId, AuctionStatus status)
    {
        return _auctionReadRepository.GetAuctionsByUserIdAndStatus(userId, status);
    }
    /// <summary>
    /// Recupera todas las subastas creadas por un usuario específico.
    /// Consulta el repositorio de lectura utilizando el identificador de usuario proporcionado para obtener sus subastas.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyas subastas se desean recuperar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> correspondientes al usuario indicado.
    /// </returns>
    public async Task<List<Auction>> GetMyAuctionsAsync(Guid userId)
    {
        var auctions = await _auctionReadRepository.GetAuctionsByUserId(userId);
        return auctions;
    }
    /// <summary>
    /// Registra una nueva subasta en el sistema persistente.
    /// Invoca el repositorio de escritura para agregar la subasta y devuelve el resultado de la operación.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los datos completos de la subasta que se desea registrar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve el código de estado HTTP que indica el resultado del intento de registro.
    /// </returns>
    public async Task<HttpStatusCode> RegisterAuction(Auction auction)
    {
        var response = await _auctionWriteRepository.AddAuction(auction);
        return response;
    }
    /// <summary>
    /// Actualiza una subasta existente en el sistema persistente.
    /// Invoca el repositorio de escritura para aplicar los cambios realizados al objeto <see cref="Auction"/>.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> con la información actualizada que se desea almacenar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve el código de estado HTTP que indica el resultado de la operación de actualización.
    /// </returns>
    public async Task<HttpStatusCode> UpdateAuctionAsync(Auction auction)
    {
        var response = await _auctionWriteRepository.UpdateAuction(auction);
        return response;
    }
}
