namespace microservicio_reclamos.Requests;

public record CreateComplaintRequest(string reason, string description, IFormFile? evidence);
