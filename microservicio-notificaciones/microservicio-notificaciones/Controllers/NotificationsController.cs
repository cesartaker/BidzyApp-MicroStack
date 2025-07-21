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
        /// Endpoint de entrada para el envío de una notificación de correo electrónico.
        /// </summary>
        /// <param name="command">Contiene los campos requeridos de la solicitud.</param>
        /// <returns>Devuelve una respuesta Http con un mensaje.</returns>
        [HttpPost("auction/send/email")]
        public async Task<IActionResult> SendEmailNotification([FromBody] SendEmailNotificationCommand command)
        {
            _logger.LogInformation("Iniciando envío de notificación");

            try
            {
                var response = await _mediator.Send(command);
                _logger.LogDebug("Comando de envío de notificación enviado: {@Command}", command);
                return Ok("Notifiación de correo electrónico enviada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la notificación");
                return StatusCode(500, "Error interno del servidor al enviar la notificación");
            }   
        }

    }
}
