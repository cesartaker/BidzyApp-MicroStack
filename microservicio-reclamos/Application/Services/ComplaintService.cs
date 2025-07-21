using Application.Contracts.Respositories;
using Application.Contracts.Services;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IMongoComplaintWriteRepository _complaintWriteRepository;
    private readonly IMongoComplaintReadRespository _complaintReadRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public ComplaintService(IMongoComplaintReadRespository readRespository, IMongoComplaintWriteRepository writeRepository,
        ICloudinaryService cloudinaryService, INotificationService notificationService,IUserService userService)
    {
        _cloudinaryService = cloudinaryService;
        _complaintReadRepository = readRespository;
        _complaintWriteRepository = writeRepository;
        _notificationService = notificationService;
        _userService = userService;
    }
    /// <summary>
    /// Obtiene una lista de reclamos filtrados por estado y los transforma
    /// en una colección de objetos <see cref="ComplaintCreatedDto"/>.
    /// </summary>
    /// <param name="status">Estado por el cual se filtrarán los reclamos (ej. Pending, Resolved).</param>
    /// <returns>Una lista de DTOs con los datos de los reclamos que coinciden con el estado especificado.</returns>
    public async Task<List<ComplaintCreatedDto>> GetComplaintsAsync(ComplaintStatus status)
    {
        var complaints = await _complaintReadRepository.GetComplaintsByStatus(status);
        var complaintDtos = complaints.Select(c => new ComplaintCreatedDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Reason = c.Reason,
            Description = c.Description,
            EvidenceUrl = c.EvidenceUrl,
            Status = c.Status,
            CreatedAt = c.CreatedAt
        }).ToList();

        return complaintDtos;
    }
    /// <summary>
    /// Registra un nuevo reclamo de usuario con la información proporcionada,
    /// carga el archivo de evidencia si se incluye, y guarda el reclamo con estado pendiente en el repositorio.
    /// </summary>
    /// <param name="userId">Identificador único del usuario que realiza el reclamo.</param>
    /// <param name="reason">Motivo principal del reclamo.</param>
    /// <param name="description">Descripción detallada del reclamo.</param>
    /// <param name="evidence">Archivo de evidencia opcional que respalda el reclamo.</param>
    /// <returns>Un objeto <see cref="Complaint"/> que representa el reclamo registrado.</returns>
    public async Task<Complaint> RegisterComplaintAsync(Guid userId, string reason, string description, IFormFile? evidence = null)
    {
        var fileUrl = string.Empty;
        if (evidence != null)
            fileUrl = await _cloudinaryService.UploadAnyFileAsync(evidence,"Evidence");

        var complaint = new Complaint(userId, reason, description, fileUrl, ComplaintStatus.Pending);
        await _complaintWriteRepository.AddComplaint(complaint);

        return complaint;
    }
    /// <summary>
    /// Envía una notificación por correo electrónico al usuario indicando que su reclamo ha sido solucionado.
    /// El mensaje incluye el motivo, descripción y solución del reclamo.
    /// </summary>
    /// <param name="complaint">Objeto <see cref="Complaint"/> que contiene la información del reclamo resuelto.</param>
    public async Task SendSolveComplaintNotification(Complaint complaint)
    {
        var email = await GetUserEmail(complaint.UserId);
        var message = $"Tu reclamo ha sido solucionado:\n" +
            $"Motivo : {complaint.Reason}.\n" +
            $"Descripción: {complaint.Description}.\n" +
            $"Solución: {complaint.Solution}.";
        if(email!=null)
            await _notificationService.SendNotificationAsync(email, message,"Reclamo Solucionado");
    }
    /// <summary>
    /// Actualiza un reclamo existente aplicando la solución proporcionada, 
    /// cambia su estado a resuelto y guarda los cambios en el repositorio.
    /// </summary>
    /// <param name="complaintId">Identificador único del reclamo que se desea actualizar.</param>
    /// <param name="solution">Descripción de la solución que se aplicará al reclamo.</param>
    /// <returns>Un objeto <see cref="Complaint"/> que representa el reclamo actualizado.</returns>
    public async Task<Complaint> UpdateComplaintAsync(Guid complaintId, string solution)
    {
        var complaint = _complaintReadRepository.GetComplaintById(complaintId);
        complaint.Resolve(solution);
        await _complaintWriteRepository.UpdateComplaint(complaint);
        return complaint;
    }
    /// <summary>
    /// Obtiene la dirección de correo electrónico asociada a un usuario específico,
    /// consultando la información a través del servicio de usuarios.
    /// </summary>
    /// <param name="userId">Identificador único del usuario.</param>
    /// <returns>Una cadena con el correo electrónico del usuario, o <c>null</c> si no se encuentra.</returns>
    public async Task<string> GetUserEmail(Guid userId)
    {
        List<Guid?> userIds = new List<Guid?> { userId };
        var users = await _userService.GetAuctionUserInformationByIds(userIds);
        return users.FirstOrDefault()?.email;
    }
    /// <summary>
    /// Obtiene todos los reclamos registrados por un usuario específico,
    /// consultando el repositorio de lectura correspondiente.
    /// </summary>
    /// <param name="userId">Identificador único del usuario.</param>
    /// <returns>Una lista de objetos <see cref="Complaint"/> asociados al usuario especificado.</returns>
    public Task<List<Complaint>> GetComplaintsByUserId(Guid userId)
    {
        return _complaintReadRepository.GetComplaintsByUserId(userId);
    }
}
