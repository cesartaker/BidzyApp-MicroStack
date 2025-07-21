using System.Net;
using System.Text.Json;
using Application.DTOs;
using DotNetEnv;
using Newtonsoft.Json;
using RestSharp;
using Domain.ValueObjects;
using Newtonsoft.Json.Linq;
using System.Security.Authentication;
using Domain.Entities;
using Sprache;
using MongoDB.Driver;
using Infrastructure.ModelResponses;
using Application.Contracts.Services;
namespace Infrastructure.Services;

public class KeycloakServices : IKeycloackService
{
    private readonly IRestClient _client;
    private readonly string _baseUrl;
    private readonly string _realm;
    private readonly string _clientId;
    private readonly string _publicClientId;
    private readonly string _clientSecret;
    private readonly string _adminUser;
    private readonly string _adminPassword;
    private string _token;
    private DateTime _tokenExpiration;


    public KeycloakServices(IRestClient? client = null)
    {
        Env.Load(@"C:\Users\FAMILIA\Desktop\DS-proyecto\codigo\microservicio-usuarios\microservicio-usuarios\Infrastructure\.env");

        _baseUrl = Environment.GetEnvironmentVariable("KEYCLOAK_BASE_URL")!;
        _realm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM")!;
        _clientId = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID")!;
        _publicClientId = Environment.GetEnvironmentVariable("KEYCLOAK_PUBLIC_CLIENT_ID")!;
        _clientSecret = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET")!;
        _adminUser = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_USER")!;
        _adminPassword = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_PASSWORD")!;
        _client = client ?? new RestClient(_baseUrl);
        _token = string.Empty;
        _tokenExpiration = DateTime.Now;
    }

    public async Task<string?> GetClientTokenAsync()
    {
        try
        {
            var request = new RestRequest($"/realms/{_realm}/protocol/openid-connect/token", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("grant_type", "client_credentials");

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                string errorMessage = string.IsNullOrEmpty(response.Content) ? "Sin contenido en la respuesta." : response.Content;

                throw response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new AuthenticationException($"Error de autenticación: {response.StatusCode} - {errorMessage}"),
                    HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode} - {errorMessage}"),
                    HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode} - {errorMessage}"),
                    _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode} - {errorMessage}")
                };
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(response.Content! ?? String.Empty);
            _tokenExpiration = DateTime.UtcNow.AddSeconds(token!.expires_in);
            return token?.access_token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] getAdminTokenAsync {ex.Message}");
            throw;
        }
    }
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _tokenExpiration)
            {
                return _token;
            }

            var response = await GetClientTokenAsync();
            if (string.IsNullOrEmpty(response))
            {
                throw new InvalidOperationException("No se pudo obtener un token valido de keycloak ");
            }
            _token = response;
            return _token;
        }
        catch (Exception )
        {  
            throw;
        }
    }
    public async Task<string?> GetAdminTokenAsync()
    {
        try
        {
            var request = new RestRequest($"/realms/{_realm}/protocol/openid-connect/token", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _publicClientId);
            request.AddParameter("username", _adminUser);
            request.AddParameter("password", _adminPassword);
            request.AddParameter("grant_type", "password");
            request.AddParameter("scope", "openid");

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new InvalidOperationException($"Error obtendiendo token de Keycloak: {response.StatusCode} - {response.Content}");
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(response.Content! ?? String.Empty);
            _tokenExpiration = DateTime.UtcNow.AddSeconds(token!.expires_in);
            return token?.access_token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] getAdminTokenAsync {ex.Message}");
            throw;
        }
    }
    public async Task<string?> GetUserTokenAsync(string username, string password)
    {
        var request = new RestRequest($"/realms/{_realm}/protocol/openid-connect/token", Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("client_id", _publicClientId);
        request.AddParameter("username", username);
        request.AddParameter("password", password);
        request.AddParameter("grant_type","password");
        request.AddParameter("scope", "openid");

        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
                throw new UnauthorizedAccessException($"Error obtendiendo token de Keycloak: {response.StatusCode} - {response.Content}");
        }

        var token = JsonConvert.DeserializeObject<TokenResponse>(response.Content! ?? String.Empty);  
        return token?.access_token;
    }
    public async Task<Guid> GetUserIdAsync(string email)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/users?email={email}", Method.Get);
            rest.AddHeader("Authorization", $"Bearer {token}");
            rest.AddHeader("Content-Type", "application/json");

            var response = await _client.ExecuteAsync(rest);
            if (!response.IsSuccessful)
            {
                throw new InvalidOperationException($"Error al buscar usuario: {response.StatusCode} - {response.Content}");
            }

            var user = JsonConvert.DeserializeObject<List<UserResponse>>(response.Content!);
            var userId = user!.FirstOrDefault()?.id.ToString();
            if (userId == null)
            {
                throw new HttpRequestException($"No existe ningun usuario registrado con el correo {email} : {response.StatusCode}");
            }
            return Guid.Parse(userId!);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] GetUserIdAsync {ex.Message}");

            throw;
        }

    }
    public async Task<Guid> GetRolIdAsync(string roleName)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/roles", Method.Get);
            rest.AddHeader("Authorization", $"Bearer {token}");
            rest.AddHeader("Content-Type", "application/json");

            var response = await _client.ExecuteAsync(rest);
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException($"Acceso prohibido al obtener roles de {_realm}: {response.StatusCode}");
            }

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new InvalidOperationException($"Error al obtener roles de {_realm} desde Keycloak: {response.StatusCode}");
            }


            var roles = JsonConvert.DeserializeObject<List<RoleResponse>>(response.Content!)!;
            var role = roles.FirstOrDefault(r => r.name == roleName);
            return role!.id;
        }
        catch (Exception)
        {
            throw;
        }

    }
    public async Task<string?> GetCredentialIdAsync(string userId)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/users/{userId}/credentials", Method.Get);
            rest.AddHeader("Authorization", $"Bearer {token}");
            rest.AddHeader("Content-Type", "application/json");
            
            var response = await _client.ExecuteAsync(rest);

            if(!response.IsSuccessful)
            {
                throw new InvalidOperationException($"Error en la solicitud: {response.StatusCode} - {response.Content}");
            }

            if(string.IsNullOrEmpty(response.Content))
            {
                throw new InvalidOperationException($"La respuesta del servidor está vacía.");
            }

                var credentials = JsonConvert.DeserializeObject<List<CredentialResponse>>(response.Content);
                var passwordCredential = credentials?.FirstOrDefault(c => c.type == "password");

                return passwordCredential?.id ?? throw new InvalidOperationException("No se encontró una credencial del tipo password");
        }
        catch(Exception)
        {
            throw;
        } 
    }
    public async Task<HttpStatusCode> DeleteCredentialById(string userId,string credentialId)
    {
        var token = await GetAdminTokenAsync();
        var rest = new RestRequest($"admin/realms/{_realm}/users/{userId}/credentials/{credentialId}", Method.Delete);
        rest.AddHeader("Authorization", $"Bearer {token}");
        rest.AddHeader("Content-Type", "application/json");

        var response =await _client.ExecuteAsync(rest);

        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            throw response.StatusCode switch
            {
                HttpStatusCode.Forbidden => new HttpRequestException($"La solicitud contiene parámetros inválidos:{response.StatusCode}"),
                HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode}"),
                HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode}"),
                HttpStatusCode.NotFound => new ArgumentException($"Error en la solicitud: No se encontró ninguna credencial con el id [{credentialId}]. Status code: {response.StatusCode}"),
                _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode}")
            };
        }

        return response.StatusCode;
    }
    public async Task<bool> ValidCredentials(string username, string password)
    {
        try
        {
            var result = await GetUserTokenAsync(username, password);
            if (result != null)
                return true;
        }
        catch(UnauthorizedAccessException)
        {
            throw;
        }
     
        return false;
    }
    public async Task<HttpStatusCode> CreateUserAsync(CreateUserDto request)
    {
        try
        {
            var token = await GetTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/users", Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            rest.AddHeader("Authorization", $"Bearer {token}");
            rest.AddHeader("Content-Type", "application/json");
            var userData = new
            {
                username = request.Email,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = request.Password,
                        temporary = false,
                    },
                },
            };
            rest.AddJsonBody(userData);

            var response = await _client.ExecuteAsync(rest);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                string contentMessage = string.IsNullOrEmpty(response.Content) ? "Sin contenido en respuesta.": response.Content;
                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Solicitud mal formada. Revisa los datos enviados.",
                    HttpStatusCode.Conflict => "Ya existe un usuario con este correo.",
                    HttpStatusCode.InternalServerError => "El servidor tuvo un problema interno.",
                    _ => $"Error desconocido. Status Code: {response.StatusCode}"
                };

                throw new HttpRequestException($"{errorMessage}. Content: {response.Content}");
            }

            return response.StatusCode;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] CreateUserAsync : {ex.Message}");
            throw;
        }
    }
    public async Task <HttpStatusCode> AssingRoleToUserAsync(Guid userId, Guid roleId, string roleName) 
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var request = new RestRequest($"admin/realms/{_realm}/users/{userId}/role-mappings/realm", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(new[]
            {
                new {id= roleId.ToString(), name = roleName}
            });

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new InvalidOperationException($"Error al asignar el rol {roleName} al usuario {userId}: {response.StatusCode}");
            }

            return response.StatusCode;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task <HttpStatusCode> DeleteUserAsync(Guid userId)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"/admin/realms/{_realm}/users/{userId}",Method.Delete);
            rest.AddHeader ("Content-Type", "application/json");
            rest.AddHeader("Authorization", $"Bearer {token}");
            var response = await _client.ExecuteAsync(rest);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.Forbidden => new HttpRequestException($"La solicitud contiene parámetros inválidos:{response.StatusCode}"),
                    HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode}"),
                    HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode}"),
                    _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode}")
                };
            }
            return response.StatusCode;
        }
        catch(Exception)
        {
            throw;
        }
    }
    public async Task <HttpStatusCode> UpdateUserPasswordAsync(string userId, string password)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/users/{userId}/reset-password", Method.Put);
            rest.AddHeader("Authorization", $"Bearer {token}");
            rest.AddHeader("Content-Type", "application/json");
            rest.AddJsonBody (new
            {
                type = "password",
                value = password,
                temporary = false
            });

            var response = await _client.ExecuteAsync(rest);

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.Forbidden => new HttpRequestException($"La solicitud contiene parámetros inválidos:{response.StatusCode}"),
                    HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode}"),
                    HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode}"),
                    _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode}")
                };
            }
            return response.StatusCode;
        }
        catch
        {
            throw;
        }
    }
    public async Task<HttpStatusCode> SendVerifyEmailToUserAsync(Guid userId)
    {
        try
        {
            var token = await GetAdminTokenAsync();
            var rest = new RestRequest($"admin/realms/{_realm}/users/{userId}/send-verify-email", Method.Put);
            rest.AddHeader("Content-Type", "application/json");
            rest.AddHeader("Authorization", $"Bearer {token}");

            var response = await _client.ExecuteAsync(rest);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.Forbidden => new HttpRequestException($"La solicitud contiene parámetros inválidos:{response.StatusCode}"),
                    HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode}"),
                    HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode}"),
                    _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode}")
                };
            }
            return response.StatusCode;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task <HttpStatusCode> SendEmailForResetPassword(Guid userId)
    {
        var token = await GetAdminTokenAsync();
        var rest = new RestRequest($"admin/realms/{_realm}/users/{userId}/reset-password-email", Method.Put);
        rest.AddHeader("Content-Type", "application/json");
        rest.AddHeader("Authorization", $"Bearer {token}");
        var body = new { actions = new string[] { "UPDATE_PASSWORD" } };
        rest.AddJsonBody (body);
        var response = await _client.ExecuteAsync(rest);
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            throw response.StatusCode switch
            {
                HttpStatusCode.Forbidden => new HttpRequestException($"La solicitud contiene parámetros inválidos:{response.StatusCode}"),
                HttpStatusCode.InternalServerError => new HttpRequestException($"Error interno en el servidor de Keycloak: {response.StatusCode}"),
                HttpStatusCode.BadRequest => new ArgumentException($"Error en la solicitud: Verifica los parámetros enviados. Status code: {response.StatusCode}"),
                HttpStatusCode.NotFound => new HttpRequestException($"El recurso no fue encontrado, o esta deshabiitado"),
                _ => new Exception($"Error inesperado obteniendo token de Keycloak: {response.StatusCode}")
            };
        }

        return response.StatusCode;

    }

    public async Task<HttpStatusCode> AssingUserToGroupAsync(Guid userId, string groupName)
    {
        var groupId = await GetGroupIdAsync(groupName);
        var token = await GetAdminTokenAsync();

        var request = new RestRequest($"admin/realms/{_realm}/users/{userId}/groups/{groupId}", Method.Put);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("Content-Type", "application/json");
        
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Error al asignar grupo: {response.StatusCode} - {response.Content}");
        }

        return response.StatusCode;
    }

    public async Task<string?> GetGroupIdAsync(string groupName)
    {
        var token = await GetAdminTokenAsync();
        var rest = new RestRequest($"admin/realms/{_realm}/groups", Method.Get);
        rest.AddHeader("Authorization", $"Bearer {token}");
        rest.AddHeader("Content-Type", "application/json");

        var response = await _client.ExecuteAsync(rest);

        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Error en la solicitud: {response.StatusCode} - {response.Content}");
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            throw new InvalidOperationException($"La respuesta del servidor está vacía.");
        }

        var groups = JsonConvert.DeserializeObject<List<GroupRepresentation>>(response.Content);
        var group = groups?.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        return group?.Id;
    }
}