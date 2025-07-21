using System;

namespace Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un permiso ingresado no es válido o no existe.
/// </summary>
public class InvalidPermissionException : Exception
{
    public InvalidPermissionException(string permissionName)
        : base($"El permiso '{permissionName}' no es válido o no existe en la base de datos.")
    {
    }
}