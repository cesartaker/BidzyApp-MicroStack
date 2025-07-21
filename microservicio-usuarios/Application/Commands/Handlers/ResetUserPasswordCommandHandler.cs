using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Exceptions;
using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class ResetUserPasswordCommandHandler: IRequestHandler<ResetUserPasswordCommand, HttpStatusCode>
{
    
    private readonly IKeycloackService _keycloackService;
    private readonly IUserService _userService;
    private readonly IUserAuditService _userAuditService;
    private readonly IValidator<ResetUserPasswordCommand> _validator;

    public ResetUserPasswordCommandHandler(IKeycloackService keycloackService, IUserService userService,
        IUserAuditService userAuditService, IValidator<ResetUserPasswordCommand> validator)
    {
        _keycloackService = keycloackService;
        _userService = userService;
        _validator = validator;
        _userAuditService = userAuditService;
    }
    /// <summary>
    /// Maneja el comando <see cref="ResetUserPasswordCommand"/> para iniciar el proceso de recuperación de contraseña.
    /// Valida la solicitud, verifica la existencia del usuario, envía el correo de recuperación a través de Keycloak
    /// y registra la acción en el historial de auditoría del usuario.
    /// </summary>
    /// <param name="request">Comando que contiene el correo electrónico del usuario para la recuperación de contraseña.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Código <see cref="HttpStatusCode"/> que indica el resultado del intento de envío del correo de recuperación.
    /// </returns>
    /// <exception cref="ValidationException">Se lanza si la validación del comando falla.</exception>
    /// <exception cref="NotRegisteredUserException">Se lanza si el usuario no está registrado en el sistema.</exception>
    /// <exception cref="Exception">Se lanza si ocurre un error inesperado durante el procesamiento.</exception>
    public async Task<HttpStatusCode> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(", ", errors));
            }

            var registeredUser = await _userService.UserExistAsync(request.Email);
            if (!registeredUser)
                throw new NotRegisteredUserException(request.Email);

            var user = await _userService.GetUser(request.Email);

            var result = await _keycloackService.SendEmailForResetPassword(user.Id);
            await _userAuditService.AddToHistory(user.Id, "Recuperación de Contraseña");

            return result;
        }
        catch (Exception)
        {
            throw; 
        }
    }
}

