using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.MongoDTOs;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class UserUpdatedEventNotificationHandler: INotificationHandler<UserUpdatedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public UserUpdatedEventNotificationHandler( ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    /// <summary>
    /// Maneja el evento <see cref="UserUpdatedEventNotification"/> y publica la información actualizada del usuario
    /// en la cola <c>update_users_queue</c>. Construye un objeto <see cref="MongoUserDto"/> con los datos modificados
    /// y lo envía a través del proveedor de endpoints configurado.
    /// </summary>
    /// <param name="notification">Notificación del evento que contiene los datos actualizados del usuario.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    public async Task Handle(UserUpdatedEventNotification notification, CancellationToken cancellationToken)
    {
        var user = new MongoUserDto(
            notification.User.Id,
            notification.User.FirstName,
            notification.User.MiddleName,
            notification.User.LastName,
            notification.User.SecondLastName,
            notification.User.Email,
            notification.User.PhoneNumber,
            notification.User.Address,
            notification.User.RoleId,
            notification.Rolename);

        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:update_users_queue"));
        await sendEndpoint.Send(user);
    }
}
