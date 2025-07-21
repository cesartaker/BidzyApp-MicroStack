using System;

namespace Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un permiso ya tiene el estado solicitado.
/// </summary>
public class PermissionAlreadySetException : Exception
{
    public PermissionAlreadySetException(string roleName, string permissionName, string enable)
        : base($"El permiso '{permissionName}' en el rol '{roleName}' ya está configurado como '{enable}'.")
    {
    }
}
