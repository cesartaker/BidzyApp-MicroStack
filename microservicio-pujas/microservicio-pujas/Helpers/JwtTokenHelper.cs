using System.IdentityModel.Tokens.Jwt;

namespace bids_api.Helpers;

public static class JwtTokenHelper
{
    /// <summary>
    /// Extrae el identificador de usuario desde un token JWT accediendo al claim <c>"sub"</c>.
    /// Si el token no contiene dicho claim, lanza una excepción <see cref="ArgumentException"/> indicando que el ID no está presente.
    /// </summary>
    /// <param name="token">
    /// Cadena <see cref="string"/> que representa el token JWT codificado.
    /// </param>
    /// <returns>
    /// Valor <see cref="string"/> correspondiente al claim <c>"sub"/> que representa el identificador único del usuario.
    /// </returns>
    public static string ExtractUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
               ?? throw new ArgumentException("Token no contiene el claim 'sub' con el ID de usuario.");
    }
    /// <summary>
    /// Extrae el nombre de usuario desde un token JWT, accediendo al claim <c>"name"</c>.
    /// Si el token no contiene dicho claim, lanza una excepción <see cref="ArgumentException"/> indicando su ausencia.
    /// </summary>
    /// <param name="token">
    /// Cadena JWT codificada (<see cref="string"/>), que contiene los claims del usuario.
    /// </param>
    /// <returns>
    /// El nombre del usuario (<see cref="string"/>), obtenido desde el claim <c>"name</c> del token.
    /// </returns>
    public static string ExtractUserNameFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value
               ?? throw new ArgumentException("Token no contiene el claim 'name' con el nombre de usuario.");
    }
}
