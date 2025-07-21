using System.IdentityModel.Tokens.Jwt;
using Application.Command;
using MediatR;
using microservicio_reclamos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace microservicio_reclamos.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    public class ComplaintsController : ControllerBase
    {

        private readonly ILogger<ComplaintsController> _logger;
        private readonly IMediator _mediator;

        public ComplaintsController(ILogger<ComplaintsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        /// <summary>
        /// Endpoint para crear un reclamo.
        /// </summary>
        /// <param name="request">Record con los campos asociados a la solicitud.</param>
        /// <returns>Regresa una respuesta Http con el contenido del reclamo.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateComplaint([FromForm] CreateComplaintRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token." });
            }

            var command = new CreateComplaintCommand(Guid.Parse(userId), request.reason, request.description, request.evidence);

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }

        }
        /// <summary>
        /// Endpoint para obtener la lista de reclamos pendientes por resolver.
        /// </summary>
        /// <returns>Regresa una respuesta Http con la lista de reclamos pendientes</returns>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingComplaints()
        {
            
            try
            {
                var result = await _mediator.Send(new GetPendingComplaintsCommand());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }

        }
        /// <summary>
        /// Endpoint para enviar la resolución de un reclamo.
        /// </summary>
        /// <param name="command">Record con los campos asociados a la solicitud.</param>
        /// <returns>Regresa una respuesta Http con el detalle de reclamo y su resolución</returns>
        [HttpPost("solve")]
        public async Task<IActionResult> SolveComplaint([FromBody] SolveComplaintCommand command)
        {           
            try
            {
                var result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound(new { message = "Complaint not found or already solved." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }

        }
        /// <summary>
        /// Endpoint para solicitar toda la lista de reclamos del usuario.
        /// </summary>
        /// <returns>Regresa una respuesta Http con el la lista de reclamos asociados al usuario</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            try
            {
                var result = await _mediator.Send(new GetMyComplaintsCommand(Guid.Parse(userId)));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }

        }
    }
}
