using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Builders;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Application.Dtos.NotificationQuery;
using Domain.Entities;

namespace Application.Builders;
public class NotificationBuilder : INotificationBuilder
{
    private readonly IAuctionService _auctionService;
    private readonly IBidsService _bidsService;
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    public NotificationBuilder(IAuctionService auctionService, IBidsService bidsService,
        IProductService productService, IUserService userService)
    {
        _auctionService = auctionService;
        _bidsService = bidsService;
        _productService = productService;
        _userService = userService;
    }
    /// <summary>
    /// Construye una notificación relacionada con una subasta específica.
    /// Recupera los datos necesarios de la subasta, el producto asociado, los usuarios participantes y la puja relevante,
    /// y los combina en un objeto <see cref="NotificationDto"/>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) para la cual se generará la notificación.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{NotificationDto}"/> que representa la operación asincrónica:
    /// devuelve el contenido completo de la notificación construida.
    /// </returns>
    public async Task<NotificationDto> BuildNotificationAsync(Guid auctionId)
    {
        var auction = await _auctionService.GetAuctionByIdAsync(auctionId);
        var product = await GetProduct(auction.ProductId);
        var users = await GetUsers(auction);
        var bid = await GetBid(auctionId);

        return BuildNotification(auction, product, users, bid);
    }
    /// <summary>
    /// Obtiene la información básica de un producto específico a partir de su identificador.
    /// Realiza una consulta al servicio de productos utilizando una lista con un solo elemento.
    /// </summary>
    /// <param name="productId">
    /// Identificador único del producto (<see cref="Guid"/>) que se desea recuperar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{ProductDto}"/> que representa la operación asincrónica:
    /// devuelve un objeto <see cref="ProductDto"/> con los datos básicos del producto (ID y nombre).
    /// </returns>
    public async Task<ProductDto> GetProduct(Guid productId)
    {
        List<Guid> productIds = new List<Guid> { productId };
        var result = await _productService.GetProductsInformationByIds(productIds);
        var product = result.FirstOrDefault();

        return new ProductDto { productId = product.ProductId, productName = product.ProductName };
    }
    /// <summary>
    /// Obtiene la información de los usuarios participantes en una subasta específica, incluyendo el creador y el ganador.
    /// Realiza una consulta al servicio de usuarios utilizando los identificadores proporcionados en el objeto <see cref="Auction"/>.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los identificadores del usuario creador (<c>UserId</c>) y del ganador (<c>WinnerId</c>).
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{UserDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los datos básicos de los usuarios involucrados (ID, nombre completo y correo electrónico).
    /// </returns>
    public async Task<List<UserDto>> GetUsers(Auction auction)
    {
        List<Guid?> userIds = new List<Guid?> { auction.UserId,auction.WinnerId };
        var users = await _userService.GetAuctionUserInformationByIds(userIds);
        return users.Select(u => new UserDto { id = u.userId,  fullname = u.name, email = u.email}).ToList();
    }
    /// <summary>
    /// Obtiene la puja más alta realizada en una subasta específica.
    /// Consulta el servicio de pujas por el identificador de la subasta y determina la de mayor monto.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) cuya puja más alta se desea obtener.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{AuctionBidDto}"/> que representa la operación asincrónica:
    /// devuelve un objeto <see cref="AuctionBidDto"/> correspondiente a la puja más alta encontrada,
    /// o <c>null</c> si no existen pujas para la subasta.
    /// </returns>
    public async Task<AuctionBidDto> GetBid(Guid auctionId)
    {
        var bids = await _bidsService.GetBidsByAuctionIdAsync(auctionId);
        var highestBid = bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        return highestBid;
    }
    /// <summary>
    /// Construye una notificación personalizada para una subasta finalizada.
    /// Extrae la información del subastador y del ganador desde la lista de usuarios,
    /// y genera los mensajes correspondientes para cada parte utilizando los datos de la subasta, producto y puja.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los datos generales de la subasta finalizada.
    /// </param>
    /// <param name="product">
    /// Objeto <see cref="ProductDto"/> con la información básica del producto subastado.
    /// </param>
    /// <param name="users">
    /// Lista de objetos <see cref="UserDto"/> que contienen los datos del subastador y del ganador.
    /// </param>
    /// <param name="bid">
    /// Objeto <see cref="AuctionBidDto"/> que representa la puja ganadora en la subasta.
    /// </param>
    /// <returns>
    /// Un objeto <see cref="NotificationDto"/> que contiene los mensajes y correos electrónicos personalizados
    /// tanto para el subastador como para el ganador.
    /// </returns>
    public NotificationDto BuildNotification(Auction auction, ProductDto product,
    List<UserDto> users, AuctionBidDto bid)
    {
        string auctioneerEmail = string.Empty;
        string auctioneerFullName = string.Empty;
        string winnerEmail = string.Empty;
        string winnerFullName = string.Empty;

        foreach (var user in users)
        {
            if (user.id == auction.UserId)
            {
                auctioneerEmail = user.email;
                auctioneerFullName = user.fullname;
            }
            
            if (user.id == auction.WinnerId)
            {
                winnerEmail = user.email;
                winnerFullName = user.fullname;
            }
        }

        string auctioneerMessage = $"Saludos {auctioneerFullName}, te informamos que tu subasta {auction.Name} ha finalizado. " +
            $"El usuario {winnerFullName} se ha declarado como ganador de la subasta y del producto {product.productName}. " +
            $"Ha realizado una oferta por {bid.Amount}$";

        string winnerMessage = $"Saludos {winnerFullName}, Felicidades has ganado la subasta {auction.Name} por el producto {product.productName}." +
            $"Tu oferta ganadora es de {bid.Amount}$ accede a la aplicación y dirigite al módulo de premios para formalizar el pago " +
            $"y poder reclamar tu premio";

        string auctionerSubject = $"Subasta finalizada: {product.productName}";
        string winnerSubject = $"Felicidades! has ganado la subasta: {product.productName}";

        return new NotificationDto
        {
            Id = auction.Id,
            auctioneerSubject = auctionerSubject,
            winnerSubject = winnerSubject,
            auctioneerMessage = auctioneerMessage,
            winnerEmail = winnerEmail,
            winnerMessage = winnerMessage,
            auctioneerEmail = auctioneerEmail,  
        };
    }
}
