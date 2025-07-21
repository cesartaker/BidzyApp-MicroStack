using System.IdentityModel.Tokens.Jwt;
using Application.Commands;
using Domain.Enums;
using MassTransit.MessageData.PropertyProviders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Producto.API.Requests;

namespace Producto.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController: ControllerBase
{
    public readonly IMediator _mediator;
    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Registra un nuevo producto en el sistema.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="RegisterProductRequest"/> con los datos del producto: nombre, descripción, precio base, categoría e imagen.
    /// </param>
    /// <returns>
    /// Un <see cref="IActionResult"/> que contiene el producto registrado si la operación es exitosa,
    /// o un mensaje de error en caso contrario.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> RegisterProduct([FromForm] RegisterProductRequest request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }

        var command = new RegisterProductCommand(Guid.Parse(userId),request.Name,request.Description,
            request.BasePrice, request.Category, request.Image);

        try
        {
            var product = await _mediator.Send(command);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    /// <summary>
    /// Recupera los productos del usuario autenticado, filtrados por estado.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="GetProductsRequest"/> que contiene el estado de los productos a consultar.
    /// </param>
    /// <returns>
    /// Un <see cref="IActionResult"/> con la lista de productos obtenidos si se realiza correctamente,
    /// o un mensaje de error si ocurre alguna excepción.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsRequest request)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("El token no contiene el ID del usuario");
        }

        var command = new GetProductsCommand(Guid.Parse(userId), request.productStatus);

        try
        {
            var products = await _mediator.Send(command);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    /// <summary>
    /// Recupera una colección de productos por sus identificadores.
    /// </summary>
    /// <param name="command">
    /// Comando <see cref="GetProductsByIdCommand"/> que contiene los IDs de los productos a buscar.
    /// </param>
    /// <returns>
    /// Un <see cref="IActionResult"/> con la colección de productos si la operación es exitosa,
    /// o un mensaje de error en caso de falla.
    /// </returns>
    [HttpPost("batch")]
    public async Task<IActionResult> GetProductsById([FromBody] GetProductsByIdCommand command)
    {
        try
        {
            var products = await _mediator.Send(command);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    /// <summary>
    /// Actualiza los datos de un producto existente.
    /// </summary>
    /// <param name="command">
    /// Comando <see cref="UpdateProductCommand"/> que contiene la información actualizada del producto.
    /// </param>
    /// <returns>
    /// Un <see cref="IActionResult"/> con mensaje de éxito si se actualiza correctamente,
    /// o un mensaje de error si ocurre una excepción.
    /// </returns>
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return Ok("Producto actualizado correctamente.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
