using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Events.InternalEvents;
using Domain.Entities;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

public class AddProductConsumer : IConsumer<Product>
{
    public readonly IMongoProductReadRepository _repository;

    public AddProductConsumer(IMongoProductReadRepository mongoProductReadRepository)
    {
        _repository = mongoProductReadRepository;
    }
    /// <summary>
    /// Consume un mensaje de tipo <see cref="Product"/> recibido desde una cola de mensajería.
    /// Reconstruye el objeto de producto y lo guarda en el repositorio correspondiente.
    /// </summary>
    /// <param name="context">
    /// Contexto de consumo que contiene el mensaje <see cref="Product"/> con los datos recibidos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de persistencia del producto.
    /// </returns>
    public async Task Consume(ConsumeContext<Product> context)
    {
        var product = new Product(
            context.Message.Id,
            context.Message.AuctioneerId,
            context.Message.Name,
            context.Message.Description,
            context.Message.BasePrice,
            context.Message.Category,
            context.Message.ImageUrl,
            context.Message.Status
        );

        var result = await _repository.AddProduct(product);
    }
}
