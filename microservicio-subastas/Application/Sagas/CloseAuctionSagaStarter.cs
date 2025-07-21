using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using Application.Contracts.Sagas;
using MediatR;

namespace Application.Sagas;

public class CloseAuctionSagaStarter : ICloseAuctionSagaStarter
{
    public readonly IMediator _mediator;

    public CloseAuctionSagaStarter(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Inicia la secuencia de eventos (saga) tras la finalización de una subasta.
    /// Envía comandos para cerrar la subasta, generar notificaciones correspondientes y crear el pago asociado.
    /// Ejecuta los pasos en orden mediante el mediador para asegurar el flujo completo del proceso.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se va a procesar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la ejecución asincrónica de la saga completa.
    /// </returns>
    public async Task StartSagaAsync(Guid auctionId)
    {
        await _mediator.Send(new CloseAuctionCommand(auctionId));
        await _mediator.Send(new SendNotificationCommand(auctionId));
        await _mediator.Send(new CreatePaymentCommand(auctionId));
    }
}
