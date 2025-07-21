using System;

namespace Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un rol ingresado no es válido o no existe.
/// </summary>
public class InvalidRoleNameException : Exception
{
    public InvalidRoleNameException(string roleName)
        : base($"El rol '{roleName}' no es válido o no existe en la base de datos.")
    {
    }
}
