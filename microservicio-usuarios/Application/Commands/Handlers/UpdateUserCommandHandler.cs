using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.DTOs;
using Application.Events.InternalEvents;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Commands.Handlers;

public class UpdateUserCommandHandler: IRequestHandler<UpdateUserCommand,UpdatedUserResponseDto?>
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IUserAuditService _userAuditService;

    public UpdateUserCommandHandler(IMediator mediator, IUserService userService,IUserAuditService userAuditService, IValidator<UpdateUserCommand> validator)
    {
        _mediator = mediator;
        _userService = userService;
        _userAuditService = userAuditService;
        _validator = validator;
    }
    /// <summary>
    /// Maneja el comando <see cref="UpdateUserCommand"/> para actualizar los datos personales de un usuario.
    /// Valida la solicitud, verifica la existencia del usuario, obtiene los datos actuales,
    /// construye un nuevo objeto de usuario con los cambios requeridos y lo actualiza en el sistema.
    /// Publica una notificación del evento si la operación fue exitosa.
    /// </summary>
    /// <param name="request">Comando que contiene los datos nuevos para actualizar el perfil del usuario.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Objeto <see cref="UpdatedUserResponseDto"/> con los datos actualizados del usuario.
    /// Devuelve <c>null</c> si la operación no se completa correctamente.
    /// </returns>
    /// <exception cref="ValidationException">Se lanza si los datos del comando no cumplen con las reglas de validación.</exception>
    /// <exception cref="NotRegisteredUserException">Se lanza si el usuario no existe en el sistema.</exception>
    /// <exception cref="Exception">Se lanza ante cualquier error inesperado.</exception>
    public async Task<UpdatedUserResponseDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

            //Obtener datos de usuario
            var user = await _userService.GetUser(request.Email);

            var updatedUser = new User(
                Guid.Parse(request.UserId),
                request.FirstName ?? user.FirstName,
                request.MiddleName ?? user.MiddleName,
                request.LastName ?? user.LastName,
                request.SecondLastName ?? user.SecondLastName,
                user.Email,
                string.IsNullOrEmpty(request.PhoneNumber) ? user.PhoneNumber : new PhoneNumber(request.PhoneNumber),
                request.Address ?? user.Address,
                user!.RoleId
                );
            var result = await _userService.UpdateUserAsync(updatedUser);
            if(result == HttpStatusCode.OK)
            {
                await _mediator.Publish(new UserUpdatedEventNotification(updatedUser, user.RoleName));
               

            }
            return new UpdatedUserResponseDto(updatedUser.FirstName,updatedUser.MiddleName,updatedUser.LastName,updatedUser.SecondLastName,
                updatedUser.PhoneNumber!.ToString(),updatedUser.Address);
        }
        catch(Exception)
        {
            throw;
        }
    }

}
