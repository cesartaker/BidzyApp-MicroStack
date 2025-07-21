
using System.Net;
using Application.Contracts.Services;
using Application.Dtos;
using Application.Events.InternalEvents;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class CreateAuctionCommandHandler: IRequestHandler<CreateAuctionCommand,CreatedAuctionResponseDto>
{
    private readonly IAuctionService _auctionService;
    private readonly IValidator<CreateAuctionCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IAuctionSheduler _auctionSheduler;
    private readonly IProductService _productService;
    public CreateAuctionCommandHandler(IAuctionService auctionService,IValidator<CreateAuctionCommand> validator,
        IMediator mediator, IAuctionSheduler auctionSheduler, IProductService productService)
    {
        _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _mediator = mediator;
        _auctionSheduler = auctionSheduler;
        _productService = productService;
    }
    /// <summary>
    /// Maneja el comando <see cref="CreateAuctionCommand"/> para registrar una nueva subasta.
    /// Valida la solicitud, crea la entidad de subasta, registra la subasta en el sistema,
    /// agenda su cierre automático y publica eventos relacionados. También actualiza el estado del producto vinculado.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="CreateAuctionCommand"/> que contiene los datos de la subasta a crear: usuario, producto, nombre,
    /// descripción, precio base, precio de reserva, fecha de finalización, paso mínimo de puja e imagen.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que permite cancelar la operación asincrónica si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{CreatedAuctionResponseDto}"/> que representa la operación asincrónica y devuelve
    /// los datos de la subasta creada en forma de <see cref="CreatedAuctionResponseDto"/>.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Se lanza si la validación del comando falla.
    /// </exception>
    /// <exception cref="Exception">
    /// Se lanza si el registro de la subasta devuelve un estado diferente a <c>HttpStatusCode.OK</c>.
    /// </exception>
    public async Task<CreatedAuctionResponseDto> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var auction = new Auction(request.userId, request.productId, request.name, request.description, request.basePrice,
            request.reservePrice, request.endDate,request.minBidStep, request.imageUrl);

        var statusCode = await _auctionService.RegisterAuction(auction);
        if (statusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Error creating auction: {statusCode}");
        }
        
        _auctionSheduler.ScheduleAuctionClose(auction.Id, auction.EndDate);
        
        await _mediator.Publish(new AuctionAddedEventNotification(auction), cancellationToken);
        await _productService.UpdateProductStatus(auction.ProductId,"Auctioning");
        return new CreatedAuctionResponseDto(auction);
    }
}

