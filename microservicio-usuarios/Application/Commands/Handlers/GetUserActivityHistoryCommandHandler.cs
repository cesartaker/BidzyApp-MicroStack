using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.DTOs;
using Application.Exceptions;
using MediatR;

namespace Application.Commands.Handlers;

public class GetUserActivityHistoryCommandHandler : IRequestHandler<GetUserActivityHistoryCommand,UserActivityHistoryResponseDto?>
{
    private readonly IUserService _userService;
    private readonly IUserAuditService _userAuditService;

    public GetUserActivityHistoryCommandHandler( IUserAuditService userAuditService, IUserService userService)
    {
        _userAuditService = userAuditService;
        _userService = userService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetUserActivityHistoryCommand"/> para obtener el historial de actividad de un usuario.
    /// Verifica si el usuario está registrado, recupera sus datos y consulta su historial mediante el servicio de auditoría.
    /// </summary>
    /// <param name="request">Comando que contiene el correo electrónico del usuario.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Un objeto <see cref="UserActivityHistoryResponseDto"/> que contiene el historial de actividad del usuario.
    /// Devuelve <c>null</c> si no hay datos disponibles.
    /// </returns>
    /// <exception cref="NotRegisteredUserException">
    /// Se lanza si el usuario no está registrado en el sistema.
    /// </exception>
    public async Task<UserActivityHistoryResponseDto?> Handle(GetUserActivityHistoryCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.UserExistAsync(request.email);
        if (!result)
        {
            throw new NotRegisteredUserException(request.email);
        }

        var user = await _userService.GetUser(request.email);
       
        var history = await _userAuditService.GetHistory(user.Id);

        return new UserActivityHistoryResponseDto(history);
    }
}
