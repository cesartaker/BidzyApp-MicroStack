using System.IdentityModel.Tokens.Jwt;
using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace microservicio_pujas.Controllers;

[ApiController]
[Route("api/bids")]
public class BidsController : ControllerBase
{
    public readonly IMediator _mediator;
    public readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public BidsController(IMediator mediator)
    {
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _mediator = mediator;
    }
    /// <summary>
    /// Endpoint HTTP <c>POST</c> que recibe un comando <see cref="GetBidsCommand"/> con el identificador de una subasta.
    /// Envía el comando mediante el patrón <c>Mediator</c> para recuperar las pujas registradas.
    /// Si la operación es exitosa, responde con <c>200 OK</c> y las pujas obtenidas;
    /// en caso de error, responde con <c>400 BadRequest</c> junto al mensaje de la excepción.
    /// </summary>
    /// <param name="command">
    /// Comando <see cref="GetBidsCommand"/> que contiene el identificador de la subasta a consultar.
    /// </param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> que representa la respuesta HTTP al cliente,
    /// incluyendo el resultado de la consulta o el mensaje de error.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> GetBids(GetBidsCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    /// <summary>
    /// Endpoint HTTP <c>POST</c> que recibe una lista de identificadores de subasta mediante un comando
    /// <see cref="GetBidsByAuctionIdsCommand"/> y retorna las pujas correspondientes.
    /// Utiliza el patrón <c>Mediator</c> para delegar la lógica de recuperación.
    /// Si la operación es exitosa, responde con <c>200 OK</c> y los datos obtenidos;
    /// en caso de error, responde con <c>400 BadRequest</c> junto al mensaje de excepción.
    /// </summary>
    /// <param name="command">
    /// Comando <see cref="GetBidsByAuctionIdsCommand"/> que contiene la lista de identificadores de subasta a consultar.
    /// </param>
    /// <returns>
    /// Un objeto <see cref="IActionResult"/> que representa la respuesta HTTP al cliente,
    /// incluyendo el resultado de la consulta o el mensaje de error.
    /// </returns>
    [HttpPost("batch")]
    public async Task<IActionResult> GetBidsByAuctionIds(GetBidsByAuctionIdsCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
}
