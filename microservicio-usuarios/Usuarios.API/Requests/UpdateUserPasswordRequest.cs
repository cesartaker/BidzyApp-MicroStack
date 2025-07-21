namespace Usuarios.API.Requests;

public record UpdateUserPasswordRequest(string oldPassword, string newPassword);
