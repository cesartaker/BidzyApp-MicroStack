using System.IdentityModel.Tokens.Jwt;
using Application.Commands;
using Domain.Entities;
using MediatR;
using microservicio_subastas.Requests;
using Microsoft.AspNetCore.Mvc;

namespace microservicio_subastas.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    public readonly IMediator _mediator;
    public readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    public AuctionController(IMediator mediator)
    {
        _mediator = mediator;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }
    /// <summary>
    /// Registra una nueva subasta para un producto especificado.
    /// Extrae el ID del usuario desde el token JWT y crea un comando con los datos recibidos.
    /// Envía el comando al mediador para ejecutar la operación y retorna el resultado.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="CreateAuctionRequest"/> que contiene los datos necesarios para la subasta: nombre, descripción,
    /// precio base, precio de reserva, fecha de finalización, paso mínimo de puja y URL de imagen.
    /// </param>
    /// <param name="productId">
    /// Identificador único del producto (<see cref="string"/>) asociado a la subasta, recibido como parámetro en la consulta.
    /// </param>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa el resultado de la operación:
    /// una subasta registrada si es exitoso, o un mensaje de error si ocurre una excepción o el token es inválido.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> RegisterAuction([FromForm] CreateAuctionRequest request, [FromQuery] string productId)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }

        var command = new CreateAuctionCommand(Guid.Parse(userId),Guid.Parse(productId), request.name, request.description,
            request.basePrice, request.reservePrice, request.endDate,request.minBidStep,request.imageUrl);

        try
        {
            var auction = await _mediator.Send(command);
            return Ok(auction);
        }
        catch (Exception ex)
        {
            return BadRequest(new {Message = "Ocurrió un error inesperdo", detail = ex.Message});
        }

    }
    /// <summary>
    /// Recupera la lista de subastas creadas por el usuario autenticado.
    /// Extrae el ID del usuario desde el token JWT incluido en los encabezados de la solicitud.
    /// Envía un comando al mediador con el ID del usuario para obtener sus subastas.
    /// </summary>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
    /// devuelve un objeto <see cref="OkObjectResult"/> con las subastas del usuario si tiene éxito,
    /// o un objeto <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción.
    /// </returns>
    [HttpGet("mine")]
    public async Task<IActionResult> GetmyAuctions()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
       
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }

        try
        {
            var command = new GetMyAuctionsCommand(Guid.Parse(userId));
            var auctions = await _mediator.Send(command);
            return Ok(auctions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }
    /// <summary>
    /// Recupera todas las subastas activas del sistema.
    /// Crea y envía el comando <see cref="GetActiveAuctionsCommand"/> al mediador para ejecutar la consulta.
    /// </summary>
    /// <returns>
    /// Una <see cref="Task{IActionResult}"/> que representa el resultado de la operación:
    /// devuelve un objeto <see cref="OkObjectResult"/> con la lista de subastas activas si es exitosa,
    /// o un objeto <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción.
    /// </returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAuctions()
    {

      
        try
        {
            var command = new GetActiveAuctionsCommand();
            var auctions = await _mediator.Send(command);
            return Ok(auctions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }
    /// <summary>
    /// Recupera los resultados de subastas para el usuario autenticado, filtrados por estado.
    /// Extrae el ID del usuario desde el token JWT incluido en los encabezados de la solicitud.
    /// Construye y envía un comando con los estados solicitados al mediador para obtener los resultados.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="GetAuctionResultsRequest"/> que contiene una lista de estados por los que se desea filtrar los resultados.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
    /// devuelve un objeto <see cref="OkObjectResult"/> con los resultados de la subasta si es exitoso,
    /// o un <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción o si el token es inválido.
    /// </returns>
    [HttpPost("results")]
    public async Task<IActionResult> GetAuctionResults([FromBody] GetAuctionResultsRequest request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }
        try
        {
            var command = new GetAuctionResultsCommand(Guid.Parse(userId),request.statuses);
            var results = await _mediator.Send(command);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }
    
    ///<summary>
    /// Marca una subasta como completada.
    /// Recibe el identificador de la subasta a actualizar y envía el comando correspondiente al mediador.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="SetAuctionCompletedRequest"/> que contiene el ID de la subasta a marcar como completada.
    /// </param>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa el resultado de la operación:
    /// retorna <see cref="OkResult"/> si la operación es exitosa, o un <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción.
    /// </returns>
    [HttpPost("set-status/completed")]
    public async Task<IActionResult> SetAuctionCompleted([FromBody] SetAuctionCompletedRequest request)
    {
        try
        {
            var command = new SetAuctionCompletedCommand(request.auctionId);
            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }

    /// <summary>
    /// Recupera los premios obtenidos por el usuario autenticado.
    /// Extrae el ID del usuario desde el token JWT y envía el comando correspondiente al mediador.
    /// </summary>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
    /// retorna un objeto <see cref="OkObjectResult"/> con la lista de premios si es exitosa,
    /// o un <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción o si el token es inválido.
    /// </returns>
    [HttpGet("prizes")]
    public async Task<IActionResult> GetAuctionPrizes()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }

        try
        {
            var command = new GetPrizesCommand(Guid.Parse(userId));
            var prizes = await _mediator.Send(command);
            return Ok(prizes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }

    /// <summary>
    /// Permite al usuario autenticado reclamar un premio de una subasta ganada.
    /// Extrae el ID del usuario desde el token JWT y envía el comando correspondiente al mediador.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="ClaimPrizeRequest"/> que contiene los datos necesarios para reclamar el premio.
    /// </param>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
    /// retorna <see cref="OkResult"/> si la operación es exitosa, o un <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción o si el token es inválido.
    /// </returns>
    [HttpPost("prizes/claim")]
    public async Task<IActionResult> ClaimPrize([FromBody] ClaimPrizeRequest request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }
        try
        {
            var command = new ClaimPrizeCommand(Guid.Parse(userId), request.auctionId,
                request.receptorName, request.address, request.deliveryMethod);
            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }

    /// <summary>
    /// Confirma la recepción de un premio reclamado por el usuario.
    /// Envía el comando de confirmación al mediador para actualizar el estado del premio.
    /// </summary>
    /// <param name="command">
    /// Objeto <see cref="ConfirmPrizeClaimCommand"/> que contiene los datos necesarios para confirmar la recepción del premio.
    /// </param>
    /// <returns>
    /// Un <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
    /// retorna <see cref="NoContentResult"/> si la operación es exitosa, o un <see cref="BadRequestObjectResult"/> con detalles del error si ocurre una excepción.
    /// </returns>
    [HttpPost("prizes/claim/confirmation")]
    public async Task<IActionResult> ConfirmPrizeClaim([FromBody] ConfirmPrizeClaimCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Ocurrió un error inesperdo", detail = ex.Message });
        }
    }

}
