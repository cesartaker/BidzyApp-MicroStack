using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.DTOs;
using Domain.Enums;
using FluentValidation;
using MassTransit.Middleware;
using MediatR;

namespace Application.Commands.Handlers;

public class GetProductsCommandHandler : IRequestHandler<GetProductsCommand, GetProductsResponseDto>
{
    private readonly IProductService _productService;
    private readonly IValidator<GetProductsCommand> _validator;

    public GetProductsCommandHandler(IProductService productService, IValidator<GetProductsCommand> validator)
    {
        _productService = productService;
        _validator = validator;
    }
    /// <summary>
    /// Procesa el comando <see cref="GetProductsCommand"/> para obtener productos asociados a un subastador específico,
    /// filtrados por estado. Valida los datos de entrada antes de ejecutar la consulta.
    /// </summary>
    /// <param name="request">
    /// Comando que contiene el identificador del subastador y el estado de los productos a consultar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un objeto <see cref="GetProductsResponseDto"/> con la lista de productos obtenidos.
    /// </returns>
    public async Task<GetProductsResponseDto> Handle(GetProductsCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var status = Enum.Parse<ProductStatus>(request.status, ignoreCase:true);
        var products = await _productService.GetProductsAsync(request.auctioneerId, status);

        return new GetProductsResponseDto(products);
    }        
}
