using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Events.InternalEvents;
using Application.Mappers;
using MediatR;

namespace Application.Commands.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand,Unit>
{
    IMediator _mediator;
    IProductService _productService;

    public UpdateProductCommandHandler(IMediator mediator, IProductService productService)
    {
        _mediator = mediator;
        _productService = productService;
    }
    /// <summary>
    /// Procesa el comando <see cref="UpdateProductCommand"/> para actualizar el estado de un producto.
    /// Convierte el estado recibido a su equivalente en <see cref="ProductStatus"/> y actualiza el estado del producto en la base de datos.
    /// Publica una notificación de evento indicando que el producto fue actualizado.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene el identificador del producto a actualizar y el nuevo estado como cadena de texto.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que permite cancelar la operación de forma cooperativa.
    /// </param>
    /// <returns>
    /// Un <see cref="Task"/> que finaliza con <see cref="Unit.Value"/> una vez completada la actualización y la publicación del evento.
    /// </returns>
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var status = ProductStatusMapper.MapFromString(request.status);
        var productUpdated = await _productService.UpdateProductStatus(request.productId, status);
        await _mediator.Publish(new ProductUpdatedEventNotification(productUpdated));
        return Unit.Value;
    }
}
