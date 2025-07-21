using System.IdentityModel.Tokens.Jwt;
using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace microservicio_pagos.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {

        private readonly ILogger<PaymentsController> _logger;
        private readonly IMediator _mediator;

        public PaymentsController(ILogger<PaymentsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        /// <summary>
        /// Endpoint HTTP POST que inicia el proceso de creación de un nuevo pago.
        /// Recibe un comando <see cref="CreatePaymentCommand"/> en el cuerpo de la solicitud,
        /// lo envía a través de <see cref="_mediator"/> para su procesamiento, y retorna el resultado con una respuesta HTTP <c>200 OK</c>.
        /// </summary>
        /// <param name="command">
        /// Objeto <see cref="CreatePaymentCommand"/> que contiene los datos necesarios para generar el pago.
        /// </param>
        /// <returns>
        /// Una <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
        /// retorna un <c>Ok(result)</c> con el resultado del comando procesado.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Endpoint HTTP POST que envía un comando <see cref="SendPaymentCommand"/> para procesar una transacción de pago.
        /// El comando se despacha mediante <see cref="_mediator"/> y el resultado exitoso se retorna en una respuesta <c>200 OK</c>.
        /// En caso de excepción, se registra el error y se retorna una respuesta <c>500 Internal Server Error</c>.
        /// </summary>
        /// <param name="command">
        /// Objeto <see cref="SendPaymentCommand"/> recibido en el cuerpo de la solicitud, con los datos necesarios para ejecutar el pago.
        /// </param>
        /// <returns>
        /// Una <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
        /// retorna una respuesta HTTP con el resultado del procesamiento del comando o el código de error correspondiente.
        /// </returns>
        [HttpPost("pay")]
        public async Task<IActionResult> SendPayment([FromBody] SendPaymentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return StatusCode(500, "Internal server error");
            }

        }
        /// <summary>
        /// Endpoint HTTP GET que recupera los pagos pendientes del usuario autenticado.
        /// Extrae el identificador <c>sub</c> desde el token JWT en la cabecera <c>Authorization</c>,
        /// construye un comando <see cref="GetPendingPaymentsCommand"/> y lo envía a través de <see cref="_mediator"/>.
        /// Retorna una respuesta <c>200 OK</c> con los pagos pendientes, o <c>401 Unauthorized</c> si el token es inválido,
        /// o <c>500 Internal Server Error</c> si ocurre una excepción.
        /// </summary>
        /// <returns>
        /// Una <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
        /// retorna un resultado HTTP con los pagos pendientes o el código correspondiente si ocurre un fallo.
        /// </returns>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPaymentsByUserId()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("El token no contiene el ID del usuario");
            }

            try
            {
                var command = new GetPendingPaymentsCommand(Guid.Parse(userId));
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ID del usuario desde el token");
                return StatusCode(500, "Internal server error");
            }


        }
        /// <summary>
        /// Endpoint HTTP GET que obtiene todos los métodos de pago registrados para el usuario autenticado.
        /// Extrae el identificador del usuario (<c>sub</c>) desde el token JWT presente en la cabecera <c>Authorization</c>,
        /// construye un comando <see cref="GetPaymentMethodCommand"/> con ese identificador, y lo despacha mediante <see cref="_mediator"/>.
        /// Retorna una respuesta <c>200 OK</c> con los métodos de pago, <c>401 Unauthorized</c> si el token no contiene el ID de usuario,
        /// o <c>500 Internal Server Error</c> en caso de error inesperado.
        /// </summary>
        /// <returns>
        /// Una <see cref="Task{IActionResult}"/> que representa la operación asincrónica:
        /// retorna una respuesta HTTP con la lista de métodos de pago del usuario, o un código adecuado según el flujo.
        /// </returns>
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethodsByUserId()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("El token no contiene el ID del usuario");
            }
            try
            {
                var command = new GetPaymentMethodCommand(Guid.Parse(userId));
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los métodos de pago del usuario");
                return StatusCode(500, "Internal server error");
            }
        }

        /*  [HttpPost("payment-method")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] AddPaymentMethodCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment method");
                return StatusCode(500, "Internal server error");
            }
        }
      */
    }
}