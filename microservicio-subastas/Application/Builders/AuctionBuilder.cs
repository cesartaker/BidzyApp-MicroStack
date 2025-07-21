using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Builders;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Domain.Entities;
using Domain.Enums;

namespace Application.Builders;

public class AuctionBuilder : IAuctionBuilder
{
    private readonly IAuctionService _auctionService;
    private readonly IUserService _userService;
    private readonly IBidsService _bidsService;
    private readonly IProductService _productService;

    public AuctionBuilder(IAuctionService auctionService, IUserService userService, IBidsService bidsService, IProductService productService)
    {
        _auctionService = auctionService;
        _userService = userService;
        _bidsService = bidsService;
        _productService = productService;
    }
    /// <summary>
    /// Recupera el historial de subastas para un usuario específico, filtradas por una lista de estados.
    /// Obtiene todas las subastas correspondientes, junto con la información de ganadores, productos asociados y pujas realizadas.
    /// Combina todos los datos y construye una lista de objetos <see cref="AuctionResultDto"/> con el resultado completo.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) para quien se recuperará el historial.
    /// </param>
    /// <param name="statuses">
    /// Lista de estados (<see cref="List{AuctionStatus}"/>) por los que se filtrarán las subastas.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionResultDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los resultados completos de las subastas del usuario.
    /// </returns>
    public async Task<List<AuctionResultDto>> GetAuctionsHistory(Guid userId,List<AuctionStatus> statuses)
    {
     
        var auctions = await _auctionService.GetAuctionsByStatusesAsync(statuses);
        var auctionIds = auctions.Select(x => x.Id).Distinct().ToList();

        var WinnerIds = auctions.Where(x => x.WinnerId != Guid.Empty).Select(x => x.WinnerId).Distinct().ToList();
        var Winners = await _userService.GetAuctionUserInformationByIds(WinnerIds);

        var ProductIds = auctions.Where(x => x.ProductId != Guid.Empty).Select(x=> x.ProductId).Distinct().ToList();
        var Products = await _productService.GetProductsInformationByIds(ProductIds);

        var bids = await _bidsService.GetBidsByAuctionIdsAsync(auctionIds);

        return BuildAuctionResults(userId, auctions, Winners, Products,bids);

    }
    /// <summary>
    /// Construye una lista de resultados de subastas personalizadas para el usuario especificado.
    /// Recorre las subastas recibidas y extrae información relacionada del ganador, producto y pujas para generar una colección de <see cref="AuctionResultDto"/>.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) que se utilizará para determinar su participación en las subastas.
    /// </param>
    /// <param name="auctions">
    /// Lista de objetos <see cref="Auction"/> que representan las subastas disponibles.
    /// </param>
    /// <param name="winners">
    /// Lista de objetos <see cref="AuctionUserDto"/> que contienen información de los ganadores de subastas.
    /// </param>
    /// <param name="products">
    /// Lista de objetos <see cref="AuctionProductDto"/> que contienen información de los productos subastados.
    /// </param>
    /// <param name="bids">
    /// Lista de objetos <see cref="AuctionBidDto"/> que representan las pujas realizadas en cada subasta.
    /// </param>
    /// <returns>
    /// Una lista de <see cref="AuctionResultDto"/> que resume la información consolidada de cada subasta,
    /// incluyendo datos del producto, ganador, puja más alta y participación del usuario.
    /// </returns>
    private List<AuctionResultDto> BuildAuctionResults(Guid userId, List<Auction> auctions,
        List<AuctionUserDto> winners, List<AuctionProductDto> products, List<AuctionBidDto> bids)
    {
        var auctionResults = new List<AuctionResultDto>();
        
        foreach (var auction in auctions)
        {
            var winner = winners.FirstOrDefault(x => x.userId == auction.WinnerId);
            var product = products.FirstOrDefault(x => x.ProductId == auction.ProductId);
            var IsParticipant = bids.Any(x => x.AuctionId == auction.Id && x.BidderId == userId);
            var highestBid = bids
                .Where(x => x.AuctionId == auction.Id)
                .OrderByDescending(x => x.Amount)
                .FirstOrDefault()?.Amount ?? 0;

            auctionResults.Add(new AuctionResultDto
             {
                AuctionId = auction.Id,
                Tittle = auction.Name,
                StartDate = auction.StartDate,
                EndDate =auction.EndDate,
                Status = auction.Status.ToString(),
                ImageUrl = auction.ImageUrl,
                ProductId = product?.ProductId ?? Guid.Empty,
                ProductName = product?.ProductName?? "Producto no disponible",
                ProductDescription = product?.Description ?? "Descripcion no dispoible",
                HighestBid =highestBid,
                WinnerId = winner?.userId,
                WinnerName = winner?.name?? "Sin Ganador",
                IsParticipated =IsParticipant
            });
        }
        return auctionResults;
    }
}
