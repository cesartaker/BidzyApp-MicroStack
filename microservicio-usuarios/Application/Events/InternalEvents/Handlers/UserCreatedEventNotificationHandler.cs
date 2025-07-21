using MediatR;
using MassTransit;
using Application.Events.InternalEvents;
using Application.DTOs.MongoDTOs;


namespace Application.Events.InternalEvents.Handlers;

public class UserCreatedEventNotificationHandler: INotificationHandler<UserCreatedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public UserCreatedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="UserCreatedEventNotification"/> publicando un mensaje a la cola de usuarios.
    /// Construye un objeto <see cref="MongoUserDto"/> con la información del usuario creado
    /// y lo envía a través del proveedor del endpoint configurado.
    /// </summary>
    /// <param name="notification">Evento que contiene los datos del usuario recién creado.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    public async Task Handle(UserCreatedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:users_queue"));

        await sendEndpoint.Send(new MongoUserDto(notification.UserId,notification.FirstName, notification.MiddleName,notification.LastName,
            notification.SecondLastName,notification.Email,notification.PhoneNumber,notification.Address,notification.RoleId,
            notification.RoleName),cancellationToken);
    }
}
