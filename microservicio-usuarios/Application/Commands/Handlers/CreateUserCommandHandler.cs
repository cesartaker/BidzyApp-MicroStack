using System;
using Domain.Entities;
using Application.Events;
using Domain.ValueObjects;
using MediatR;
using FluentValidation;
using Application.Exceptions;
using Application.DTOs;
using Application.Events.InternalEvents;
using System.Net;
using MassTransit.SagaStateMachine;
using Application.Contracts.Services;


namespace Application.Commands.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,CreatedUserResponseDto?>
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IKeycloackService _keycloakService;

    public CreateUserCommandHandler(IMediator mediator, IUserService userService, IValidator<CreateUserCommand> validator, IKeycloackService keycloakService)
    {
        _userService = userService;
        _mediator = mediator;
        _validator = validator;
        _keycloakService = keycloakService;
    }
    /// <summary>
    /// Maneja el comando <see cref="CreateUserCommand"/> para crear un nuevo usuario.
    /// Realiza validaciones, verifica existencia previa, interactúa con Keycloak para crear el usuario,
    /// asignar rol y grupo, registra el usuario localmente, publica un evento de creación y envía un correo de verificación.
    /// En caso de falla transaccional, intenta revertir los cambios eliminando el usuario en Keycloak.
    /// </summary>
    /// <param name="request">Comando con los datos necesarios para crear el usuario.</param>
    /// <param name="cancellationToken">Token de cancelación para controlar el flujo asincrónico.</param>
    /// <returns>
    /// Objeto <see cref="CreatedUserResponseDto"/> si la operación fue exitosa;
    /// <c>null</c> si no se pudo completar.
    /// </returns>
    /// <exception cref="ValidationException">Se lanza si la validación de datos falla.</exception>
    /// <exception cref="RegisteredUserException">Se lanza si el usuario ya está registrado.</exception>
    /// <exception cref="Exception">
    /// Se lanza ante errores transaccionales o problemas al eliminar el usuario en Keycloak.
    /// </exception>
    public async Task<CreatedUserResponseDto?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guid? userIdException = null;
        try
        {
            // Validación de la data
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(", ", errors));
            }

            //Verifica si el usuario ya está registrado
            var registeredUser = await _userService.UserExistAsync(request.Email);
            if (registeredUser)
                throw new RegisteredUserException(request.Email);

            //Registra usuario en Keycloak
            var keycloakUser = new CreateUserDto(request.Email, request.FirstName, request.LastName, request.password);
            var keycloakResponse = await _keycloakService.CreateUserAsync(keycloakUser);
            
            //Obtener el id del user en keycloak
             var userId = await _keycloakService.GetUserIdAsync(request.Email);
            userIdException = userId;
            //Obtener el id del rol en keycloak 
            var roleId = await _keycloakService.GetRolIdAsync(request.rolName);
            //Asignar el rol al usuario en keycloak
            await _keycloakService.AssingRoleToUserAsync(userId, roleId, request.rolName);
            await _keycloakService.AssingUserToGroupAsync(userId, request.rolName);

            //Obtiene el rol 
            var role = await _userService.GetRoleAsync(request.rolName);
            if (role == null)
            {
                throw new InvalidRoleException();
            }

            var user = new User(userId,request.FirstName, request.MiddleName, request.LastName, request.SecondLastName,
                new Email(request.Email), new PhoneNumber(request.PhoneNumber), request.Address, Guid.Parse(role.PostgresID));


            var result = await _userService.AddUserAsync(user);
            if (result == HttpStatusCode.OK)
            {
                await _mediator.Publish(new UserCreatedEventNotification(user.Id, user.FirstName, user.MiddleName, user.LastName,
                    user.SecondLastName, user.Email, user.PhoneNumber, user.Address, Guid.Parse(role.PostgresID), role.Name), cancellationToken);  
            }

            await _keycloakService.SendVerifyEmailToUserAsync(userId);
           
            return new CreatedUserResponseDto(user.FirstName, user.MiddleName, user.LastName, user.SecondLastName,
                user.Email.ToString(), role.Name.ToString(), "Usuario registrado de forma exitosa");
            
        }
        catch (TransactionFailureException ex)
        {
            Console.WriteLine($"[Error] Fallo en la transacción: {ex.Message}");

            if (userIdException.HasValue && userIdException.Value != Guid.Empty)
            {
                try
                {
                    await _keycloakService.DeleteUserAsync(userIdException.Value);
                }
                catch (HttpRequestException internalException)
                {
                    Console.WriteLine($"Error al eliminar usuario en Keycloak: {internalException.Message}");
                    throw new Exception($"Fallo al intentar eliminar el usuario en Keycloak.", internalException);
                }
            }

            throw new Exception("Fallo en la transacción, posible error al eliminar usuario en Keycloak.", ex);
        }
        catch (RegisteredUserException ex)
        {
            Console.WriteLine($"[Error]: createUserComandHandler:");
            throw new ArgumentException($"Error al registrar al usuario: {ex.Message}", ex);
        }
        
    }
}
