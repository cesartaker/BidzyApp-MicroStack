namespace Usuarios.API.Controllers;

using System;
using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FluentValidation;
using System.Security.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Application.DTOs;
using Usuarios.API.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;


[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado a partir del token JWT contenido en los encabezados HTTP.
    /// Extrae el identificador único (<c>sub</c>) desde el token y utiliza ese valor para enviar el comando <see cref="GetUserInformationCommand"/>.
    /// </summary>
    /// <returns>
    /// Una respuesta HTTP <see cref="IActionResult"/> que contiene los datos del usuario si la operación es exitosa,
    /// o <see cref="BadRequest"/> en caso de error.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetUserInformation()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var keycloakUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        try
        {
            var user = await _mediator.Send(new GetUserInformationCommand(Guid.Parse(keycloakUserId)));
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    /// <summary>
    /// Endpoint HTTP POST que procesa el comando para crear un nuevo usuario. 
    /// Envía el comando al mediador y maneja posibles excepciones, incluyendo errores de validación,
    /// argumentos inválidos, fallos de autenticación, errores HTTP y fallos generales.
    /// </summary>
    /// <param name="command">Comando <see cref="CreateUserCommand"/> con los datos necesarios para crear el usuario.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> con el resultado de la operación:
    /// <c>200 OK</c> si se crea exitosamente, <c>400 BadRequest</c> para errores de validación,
    /// <c>409 Conflict</c> para conflictos, <c>401 Unauthorized</c>, <c>503 ServiceUnavailable</c>,
    /// o <c>500 InternalServerError</c> según el tipo de excepción.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        try
        {
            var user = await _mediator.Send(command);
            return Ok(new { User = user });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Message.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .ToList();

            return BadRequest(new { Message = "Error de validación", Errors = errors });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error en CreateUserCommandHandler:{ex.Message}");
            return Conflict(ex.Message);
        }
        catch (AuthenticationException ex)
        {
            Console.WriteLine($"Error de Autenticación: {ex.Message}");
            return Unauthorized(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error en la solicitud HTTP:{ex.Message}");
            return StatusCode(503, new { message = "Error de comunicación" });
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error Invalid Operation Exception: {ex.Message}");
            return StatusCode(500, new { message = $"Oops! Ocurrió un error inesperado{ex.Message}" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
            return StatusCode(500, new { message = $" {ex.Message}" });
        }

    }
    /// <summary>
    /// Endpoint HTTP PATCH que actualiza la información de un usuario autenticado.
    /// Extrae el token JWT desde el encabezado de autorización, obtiene los datos del usuario,
    /// construye el comando <see cref="UpdateUserCommand"/> y lo envía al mediador para su procesamiento.
    /// </summary>
    /// <param name="command">Objeto <see cref="UpdateUserRequest"/> con los nuevos datos del usuario.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> con el resultado de la operación:
    /// <c>200 OK</c> si la actualización fue exitosa, o <c>401 Unauthorized</c> si no se puede validar el token.
    /// </returns>
    /// <exception cref="Exception">Se lanza si ocurre un error inesperado durante la ejecución del método.</exception>
    [HttpPatch]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest command)
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var keycloakUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "No se pudo obtener el id del usuario o email" });
            }

            var updatedCommand = new UpdateUserCommand(keycloakUserId, command.FirstName, command.MiddleName, command.LastName, command.SecondLastName,
                command.PhoneNumber, command.Address, email);

            var user = await _mediator.Send(updatedCommand);
            return Ok(new { User = user });
        }
        catch (Exception) {
            throw;
        }


    }
    /// <summary>
    /// Endpoint HTTP PUT que inicia el proceso de recuperación de contraseña para un usuario.
    /// Envía el comando <see cref="ResetUserPasswordCommand"/> al mediador y gestiona 
    /// posibles errores de validación y excepciones inesperadas.
    /// </summary>
    /// <param name="command">Comando que contiene los datos necesarios para generar el link de recuperación de contraseña.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> que incluye un mensaje de éxito y datos si el proceso fue exitoso (<c>200 OK</c>),
    /// o un error de validación (<c>400 BadRequest</c>) o un error interno inesperado (<c>500 InternalServerError</c>).
    /// </returns>
    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { message = "Se ha enviado un link para la recuperación contraseña al correo suministrado",
                data = result });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Message.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .ToList();

            return BadRequest(new { Message = "Error de validación", Errors = errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Oops! Ocurrió un error inesperado {ex.Message}" });
        }

    }
    /// <summary>
    /// Endpoint HTTP PUT que permite a un usuario autenticado cambiar su contraseña.
    /// Extrae el token JWT desde el encabezado de autorización, obtiene los datos del usuario,
    /// construye el comando <see cref="UpdateUserPasswordCommand"/> y lo envía al mediador para su ejecución.
    /// </summary>
    /// <param name="request">Objeto <see cref="UpdateUserPasswordRequest"/> que contiene la contraseña actual y la nueva.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> con el resultado de la operación:
    /// <c>200 OK</c> si la contraseña fue actualizada correctamente, 
    /// o <c>400 BadRequest</c> si ocurre un error durante el procesamiento.
    /// En caso de datos faltantes en el token, se devuelve <c>401 Unauthorized</c>.
    /// </returns>
    [HttpPut("change-password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordRequest request)
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var keycloakUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (string.IsNullOrEmpty(token)||string.IsNullOrEmpty(keycloakUserId)||string.IsNullOrEmpty(email))
            {
                return Unauthorized("Hubo un problema con la autenticación");
            }

            var command = new UpdateUserPasswordCommand(keycloakUserId,request.oldPassword,request.newPassword,token,email);

            var result = await _mediator.Send(command);

            return Ok(result);

        }
        catch(Exception)
        {
            return BadRequest();
        }
    }
    /// <summary>
    /// Endpoint HTTP GET que recupera el historial de actividad de un usuario autenticado.
    /// Extrae el token JWT desde el encabezado de autorización, obtiene la información del usuario,
    /// construye el comando <see cref="GetUserActivityHistoryCommand"/> y lo envía al mediador para su ejecución.
    /// </summary>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> con el resultado de la operación:
    /// <c>200 OK</c> con el historial de actividad del usuario si se obtiene correctamente, 
    /// <c>401 Unauthorized</c> si la autenticación falla, 
    /// o <c>400 BadRequest</c> si ocurre un error inesperado.
    /// </returns>
    [HttpGet("history")]
    public async Task<IActionResult> GetUserActivityHistory()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var keycloakUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(keycloakUserId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("Hubo un problema con la autenticación");
            }

            var command = new GetUserActivityHistoryCommand(keycloakUserId, email);

            var result = await _mediator.Send(command);
            return Ok(result);

        }
        catch (Exception) {
            return BadRequest();
        }
    }
    /// <summary>
    /// Endpoint HTTP GET que obtiene el identificador único del usuario a partir de su dirección de correo electrónico.
    /// Si el parámetro de consulta <paramref name="email"/> está vacío o nulo, devuelve una respuesta <c>400 BadRequest</c>.
    /// El correo se utiliza para construir el comando <see cref="GetUserIdCommand"/>, que se envía al mediador para procesarlo.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del usuario.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> que contiene el identificador del usuario si la operación es exitosa (<c>200 OK</c>),
    /// o una respuesta <c>400 BadRequest</c> si hay un error en la entrada o en el procesamiento.
    /// </returns>
    [HttpGet("id")]
    public async Task<IActionResult> GetUserId([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest();

        try
        {
            var command = new GetUserIdCommand(email);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch(Exception) {

            return BadRequest();
        }
    }
    /// <summary>
    /// Endpoint HTTP POST que recibe un comando <see cref="GetUsersByIdCommand"/> con múltiples identificadores de usuario,
    /// y devuelve la información correspondiente a cada uno tras procesarla mediante el mediador.
    /// </summary>
    /// <param name="command">Comando que contiene la lista de identificadores de usuarios a consultar.</param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> con la lista de usuarios encontrados (<c>200 OK</c>),
    /// o una respuesta <c>400 BadRequest</c> si ocurre un error durante el procesamiento.
    /// </returns>
    [HttpPost("batch")]
    public async Task<IActionResult> GetUsersById([FromBody]GetUsersByIdCommand command)
    {
        try
        {
           
            var users = await _mediator.Send(command);
            return Ok(users);
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
   
}
