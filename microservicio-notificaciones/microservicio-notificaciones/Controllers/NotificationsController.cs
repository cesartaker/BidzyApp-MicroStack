using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace microservicio_notificaciones.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint de entrada para el env�o de una notificaci�n de correo electr�nico.
        /// </summary>
        /// <param name="command">Contiene los campos requeridos de la solicitud.</param>
        /// <returns>Devuelve una respuesta Http con un mensaje.</returns>
        [HttpPost("auction/send/email")]
        public async Task<IActionResult> SendEmailNotification([FromBody] SendEmailNotificationCommand command)
        {
            _logger.LogInformation("Iniciando env�o de notificaci�n");

            try
            {
                var response = await _mediator.Send(command);
                _logger.LogDebug("Comando de env�o de notificaci�n enviado: {@Command}", command);
                return Ok("Notifiaci�n de correo electr�nico enviada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la notificaci�n");
                return StatusCode(500, "Error interno del servidor al enviar la notificaci�n");
            }   
        }

    }
}
