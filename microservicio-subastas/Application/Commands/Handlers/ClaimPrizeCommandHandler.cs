using Application.Contracts.Services;
using Application.Events.InternalEvents;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class ClaimPrizeCommandHandler : IRequestHandler<ClaimPrizeCommand,Unit>
{
    private readonly IPrizeService _prizeService;
    private readonly IMediator _mediator;
    private readonly IAuctionService _auctionService;
    private readonly IValidator<ClaimPrizeCommand> _validator;
    public ClaimPrizeCommandHandler(IPrizeService prizeService, IMediator mediator, IAuctionService auctionService,
        IValidator<ClaimPrizeCommand> validator)
    {
        _prizeService = prizeService;
        _mediator = mediator;
        _auctionService = auctionService;
        _validator = validator;
    }
    /// <summary>
    /// Procesa el comando <see cref="ClaimPrizeCommand"/> que permite a un usuario reclamar el premio de una subasta.
    /// Valida la solicitud, registra una guía de entrega, actualiza el estado de la subasta como reclamada y publica los eventos correspondientes.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="ClaimPrizeCommand"/> que contiene los datos necesarios para reclamar el premio:
    /// identificador de subasta, nombre del receptor, dirección y método de entrega.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para detener la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Unit}"/> que representa la ejecución asincrónica del comando sin valor de retorno.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Se lanza si la validación del comando falla debido a datos incompletos o incorrectos.
    /// </exception>
    public async Task<Unit> Handle(ClaimPrizeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }
        var deliveryMethod = Enum.Parse<DeliveryMethod>(request.deliveryMethod);
        var waybill = new Waybill(request.auctionId, request.receptorName,request.address, deliveryMethod);
        await _prizeService.AddWaybillAsync(waybill);
        await _mediator.Publish(new WaybillRegisteredEvent(waybill),cancellationToken);

        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        auction.SetClaimed(true);
        await _auctionService.UpdateAuctionAsync(auction);
        await _mediator.Publish(new AuctionUpdatedEventNotification(auction), cancellationToken);

        return Unit.Value;
    }
}
