using Application.Contracts.Services;
using Application.DTOs;
using MediatR;

namespace Application.Commands.Handlers;

public class GetProductsByIdCommandHandler : IRequestHandler<GetProductsByIdCommand, List<AuctionProductDto>?>
{
    private readonly IProductService _productService;

    public GetProductsByIdCommandHandler(IProductService productService)
    {
        _productService = productService;
    }
    /// <summary>
    /// Maneja la ejecución del comando <see cref="GetProductsByIdCommand"/> para recuperar una lista de productos en formato de subasta.
    /// Delega la consulta al servicio de productos utilizando los IDs especificados.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene una colección de identificadores de productos a consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en una lista de objetos <see cref="AuctionProductDto"/> si la operación es exitosa,
    /// o `null` si no se encuentra ningún producto.
    /// </returns>
    public async Task<List<AuctionProductDto>?> Handle(GetProductsByIdCommand request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductsByIdAsync(request.productIds);
    }
}
