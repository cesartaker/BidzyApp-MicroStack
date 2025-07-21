using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Dtos.AuctionResults;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class PrizeService : IPrizeService
{
    private readonly IAuctionService _auctionService;
    private readonly IProductService _productService;
    private readonly IMongoWaybillWriteRepository _waybillWriteRepository;

    public PrizeService(IAuctionService auctionService, IProductService productService, IMongoWaybillWriteRepository mongoWaybillWriteRepository)
    {
        _auctionService = auctionService;
        _productService = productService;
        _waybillWriteRepository = mongoWaybillWriteRepository;
    }
    /// <summary>
    /// Recupera la lista de premios obtenidos por un usuario a partir de sus subastas completadas.
    /// Consulta las subastas con estado <see cref="AuctionStatus.Completed"/>, obtiene los productos asociados,
    /// y construye la colección de premios mediante la combinación de ambas fuentes.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyos premios se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{PrizeDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="PrizeDto"/> que representan los premios derivados de subastas finalizadas.
    /// </returns>
    public async Task<List<PrizeDto>> GetPrizesAsync(Guid userId)
    {
        var auctions = await _auctionService.GetAuctionsByUserIdAndStatus(userId, AuctionStatus.Completed);
        var productIds = auctions.Select(a => a.ProductId).Distinct().ToList();
        var products = await _productService.GetProductsInformationByIds(productIds);
        return BuildPrizeList(auctions, products);
    }
    /// <summary>
    /// Construye una lista de objetos <see cref="PrizeDto"/> que representan los premios obtenidos en subastas completadas.
    /// Combina la información de las subastas y de los productos asociados, generando una estructura con datos descriptivos
    /// como nombre, imagen, estado de reclamo y entrega.
    /// </summary>
    /// <param name="auctions">
    /// Lista de objetos <see cref="Auction"/> que representan las subastas finalizadas del usuario.
    /// </param>
    /// <param name="products">
    /// Lista de objetos <see cref="AuctionProductDto"/> que contienen la información adicional de los productos subastados.
    /// </param>
    /// <returns>
    /// Una lista de <see cref="PrizeDto"/> que encapsulan los datos relevantes de cada premio, combinando subasta y producto.
    /// </returns>
    public List<PrizeDto> BuildPrizeList(List<Auction> auctions, List<AuctionProductDto> products)
    {
        var productMap = products.ToDictionary(p => p.ProductId);

        var prizeList = auctions.Select(auction =>
        {
            var product = productMap.ContainsKey(auction.ProductId)
                ? productMap[auction.ProductId]
                : null;

            return new PrizeDto
            {
                auctionId = auction.Id,
                WinnerId = auction.WinnerId.Value,
                auctionName = auction.Name,
                productName = product?.ProductName ?? "N/A",
                productDescription = product?.Description ?? "Sin descripción",
                productImage = auction.ImageUrl,
                isClaimed = auction.claimed,
                isDelivered = auction.recepted
            };
        }).ToList();

        return prizeList;
    }
    /// <summary>
    /// Registra una nueva guía de entrega (<see cref="Waybill"/>) en el sistema persistente.
    /// Utiliza el repositorio de escritura para almacenar los datos asociados a la guía.
    /// </summary>
    /// <param name="waybill">
    /// Objeto <see cref="Waybill"/> que contiene la información detallada de la guía de entrega:
    /// identificador de subasta, receptor, dirección y método de envío.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de persistencia sin valor de retorno.
    /// </returns>
    public async Task AddWaybillAsync(Waybill waybill)
    {
        await _waybillWriteRepository.AddWaybill(waybill);
    }
}
