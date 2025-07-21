using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class UpdateUserPasswordCommandHandler: IRequestHandler<UpdateUserPasswordCommand, HttpStatusCode>
{
    public readonly IKeycloackService _keycloackService;
    public readonly IUserService _userService;
    public readonly IValidator<UpdateUserPasswordCommand> _validator;
    public readonly IUserAuditService _userAuditService;

    public UpdateUserPasswordCommandHandler(IKeycloackService keycloackService, IUserService userService,
        IUserAuditService userAuditService, IValidator<UpdateUserPasswordCommand> validator)
    {
        _keycloackService = keycloackService;
        _userService = userService;
        _validator = validator;
        _userAuditService = userAuditService;
    }
    /// <summary>
    /// Maneja el comando <see cref="UpdateUserPasswordCommand"/> para actualizar la contraseña de un usuario autenticado.
    /// Valida la solicitud, verifica las credenciales actuales del usuario en Keycloak, actualiza la contraseña,
    /// registra la acción en el historial de auditoría y envía una notificación por correo electrónico.
    /// </summary>
    /// <param name="request">Comando que contiene las credenciales actuales, nueva contraseña e información de usuario.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Código <see cref="HttpStatusCode"/> que representa el resultado de la operación de cambio de contraseña.
    /// </returns>
    /// <exception cref="ValidationException">Se lanza si la validación de la solicitud falla.</exception>
    /// <exception cref="UnauthorizedAccessException">Se lanza si las credenciales del usuario no son válidas.</exception>
    /// <exception cref="Exception">Se lanza ante errores inesperados durante el procesamiento.</exception>
    public async Task<HttpStatusCode> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var auth = await _keycloackService.ValidCredentials(request.username,request.oldPassword);
        if (!auth)
            throw new UnauthorizedAccessException();

        var result = await _keycloackService.UpdateUserPasswordAsync(request.userId,request.newPassword);
        await _userAuditService.AddToHistory(Guid.Parse(request.userId), "Actualización de Contraseña");
        await _userService.sendEmailToUser(request.username, "Keycloak Bidzy App", "Su contraseña se ha modificado exitosamente");
        return result;
    }

}
