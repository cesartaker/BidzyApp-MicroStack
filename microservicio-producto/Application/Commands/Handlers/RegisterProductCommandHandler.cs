
using System.Net;
using Application.Contracts.Services;
using Application.DTOs;
using Application.Events.InternalEvents;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class RegisterProductCommandHandler: IRequestHandler<RegisterProductCommand,RegisteredProductResponseDto?>
{
    private readonly IMediator _mediator;
    private readonly IProductService _productService;
    private readonly IValidator<RegisterProductCommand> _validator;
    private readonly ICloudinaryService _cloudinaryService;
    

    public RegisterProductCommandHandler(IMediator mediator, IProductService productService,
        ICloudinaryService cloudinaryService,IValidator<RegisterProductCommand> validator)
    {
        _mediator = mediator;
        _productService = productService;
        _validator = validator;
        _cloudinaryService = cloudinaryService;
        
    }
    /// <summary>
    /// Procesa el comando <see cref="RegisterProductCommand"/> para registrar un nuevo producto.
    /// Valida los datos del comando, sube la imagen al servicio en la nube, crea el producto y lo guarda en la base de datos.
    /// Publica una notificación del evento de producto agregado si la operación es exitosa.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene los datos del producto a registrar: nombre, descripción, precio base, categoría, imagen y ID del subastador.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un <see cref="RegisteredProductResponseDto"/> si el producto se registra correctamente,
    /// o `null` si la operación de guardado falla.
    /// </returns>
    public async Task<RegisteredProductResponseDto?> Handle(RegisterProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image,"products");

        var product = new Product(request.AuctioneerId,request.Name, request.Description,request.BasePrice, request.Category, imageUrl,ProductStatus.Available);
        
        var response = await _productService.AddProductAsync(product);

        if(response != HttpStatusCode.OK)
        {
            return null;
        }
        
        await _mediator.Publish(new ProductAddedEventNotification(product));
        return new RegisteredProductResponseDto(product);

    }
}
