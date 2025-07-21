using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Builders;
using Application.Contracts.Services;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Handlers;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
{

    private readonly INotificationService _notificationService;
    private readonly INotificationBuilder _notificationBuilder;
    private readonly IAuctionService _auctionService;


    public SendNotificationCommandHandler( INotificationService notificationService, 
        INotificationBuilder notificationBuilder, IAuctionService auctionService)
    {
        _notificationService = notificationService;
        _notificationBuilder = notificationBuilder;
        _auctionService = auctionService;
    }
    /// <summary>
    /// Maneja el comando <see cref="SendNotificationCommand"/> para enviar notificaciones por correo electrónico
    /// al subastador y al ganador de una subasta finalizada. Obtiene la subasta y construye el contenido de la notificación.
    /// Si el estado de la subasta es <see cref="AuctionStatus.Ended"/>, se envían los correos correspondientes.
    /// </summary>
    /// <param name="request">
    /// Comando <see cref="SendNotificationCommand"/> que contiene el identificador de la subasta cuya notificación se va a generar y enviar.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite detener la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Unit}"/> que representa la operación asincrónica sin valor de retorno.
    /// </returns>
    public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var auction = await _auctionService.GetAuctionByIdAsync(request.auctionId);
        var notification = await _notificationBuilder.BuildNotificationAsync(request.auctionId);
        if(auction.Status == AuctionStatus.Ended)
        {
            await _notificationService.SendNotificationAsync(notification.auctioneerEmail, notification.auctioneerMessage, notification.auctioneerSubject);
            await _notificationService.SendNotificationAsync(notification.winnerEmail, notification.winnerMessage, notification.winnerSubject);
        }
        return Unit.Value;
    }
}
