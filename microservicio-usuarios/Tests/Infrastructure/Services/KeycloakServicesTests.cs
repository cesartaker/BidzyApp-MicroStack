
using System.Net;
using Application.DTOs;
using Infrastructure.Services;
using Moq;
using RestSharp;
using Newtonsoft.Json;
using Infrastructure.ModelResponses;
using System.Reflection;
using Application.Contracts.Services;

namespace Usuarios.Tests.Infrastructure.Services;

public class KeycloakServicesTests
{
    #region GetClientTokenAsync
    [Fact]
    public async Task GetClientTokenAsync_ReturnsValidToken()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var expectedToken = "mocked_token";
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        //Act
        var token = await service.GetClientTokenAsync();

        //Assert
        Assert.NotNull(token);
        Assert.Equal(expectedToken, token);

    }

    [Fact]
    public async Task GetClientTokenAsync_ShouldThrowException_WhenResponseIsUnsucessfull()
    {
        //arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Error de autenticación"
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);
        //act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.GetClientTokenAsync());
    }

    [Fact]
    public async Task GetClientTokenAsync_ShouldThrowException_WhenResponseHttpErrorCode()
    {
        //arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error de autenticación"
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);
        //act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetClientTokenAsync());
    }
    #endregion
    #region GetTokenAsync
    [Fact]
    public async Task GetTokenAsync_ShouldReturnExistingToken_WhenNotExpired()
    {
        var _mockRestClient = new Mock<IRestClient>();
        var _keycloakServices = new KeycloakServices(_mockRestClient.Object);
        
        typeof(KeycloakServices).GetField("_token", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_keycloakServices, "valid_token");

        typeof(KeycloakServices).GetField("_tokenExpiration", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_keycloakServices, DateTime.UtcNow.AddMinutes(10));

        // Act
        var result = await _keycloakServices.GetTokenAsync();

        // Assert
        Assert.Equal("valid_token", result);
    }

    [Fact] 
    public async Task GetTokenAsync_ShouldFetchNewToken_WhenExpired()
    {
        var mockRestClient = new Mock<IRestClient>();
        var keycloakServices = new KeycloakServices(mockRestClient.Object);

        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        typeof(KeycloakServices).GetField("_token", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(keycloakServices, "expired_token");

        typeof(KeycloakServices).GetField("_tokenExpiration", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(keycloakServices, DateTime.UtcNow.AddMinutes(-10));

        
        mockRestClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await keycloakServices.GetTokenAsync();

        // Assert
        Assert.Equal("mocked_token", result); // Ahora se compara con el valor correcto
    }
#endregion
    #region GetAdminTokenAsync
    [Fact]
    public async Task GetAdminTokenAsync_returnsValidToken()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var expectedToken = "mocked_token";
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);
        //act
        var token = await service.GetAdminTokenAsync();
        //Assert
        Assert.NotNull(token);
        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public async Task GetAdminTokenAsync_ShouldThrowException_WhenResponseIsUnsucessfull()
    {
        //arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Error de autenticación"
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);
        //act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAdminTokenAsync());
    }

    [Fact]
    public async Task GetAdminTokenAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);

        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK, // Simula una respuesta válida pero sin contenido
            Content = null
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAdminTokenAsync());
    }

    [Fact]
    public async Task GetAdminTokenAsync_ShouldThrowException_WhenRequestFails()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ThrowsAsync(new HttpRequestException("Error en la solicitud"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetAdminTokenAsync());
    }

    #endregion
    #region GetUserTokenAsync
    [Fact]
    public async Task GetUserTokenAsync_ShouldReturnValidToken_WhenAuthenticationSucceeds()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var expectedToken = "mocked_token";
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = expectedToken }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        // Act
        var token = await service.GetUserTokenAsync("testUser", "testPassword");

        // Assert
        Assert.NotNull(token);
        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public async Task GetUserTokenAsync_ShouldThrowException_WhenAuthenticationFails()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.Unauthorized, 
            Content = "Credenciales incorrectas"
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetUserTokenAsync("invalidUser", "invalidPassword"));
    }
    [Fact]
    public async Task GetUserTokenAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var response = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = null 
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetUserTokenAsync("user", "password"));
    }

    [Fact]
    public async Task GetUserTokenAsync_ShouldThrowException_WhenRequestFails()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ThrowsAsync(new HttpRequestException("Error en la solicitud"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserTokenAsync("user", "password"));
    }
    #endregion
    #region GetUserIdAsync
    [Fact]
    public async Task GetUserIdAsync_ReturnsGuid()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        string email = "generaluser@gmail.com";
        var expectedUserId = Guid.NewGuid();
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new List<UserResponse>
            {
                new UserResponse {id = expectedUserId}
            }),
            ContentType = "application/json",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms/bidzy-app/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        //Act
        var userId = await service.GetUserIdAsync(email);
        //Assert
        Assert.Equal(expectedUserId, userId);
    }

    [Fact]
    public async Task GetUserIdAsync_ShouldThrowException_WhenResponseIsUnsucessfull()
    {
        //Arrange
        string email = "generaluser@gmail.com";
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = "Usuario No Encontrado",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = false
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms/bidzy-app/protocol/openid-connect/token")), default))
          .ReturnsAsync(tokenResponse);


        //Act
        var exception = await Record.ExceptionAsync(() => service.GetUserIdAsync(email));

        //Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task GetUserIdAsync_ShouldThrowException_WhenRequestFails()
    {
        // Arrange
        string email = "generaluser@gmail.com";
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Error en la solicitud"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms/bidzy-app/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetUserIdAsync(email));
    }
    #endregion
    #region CreateUser
    [Fact]
    public async Task CreateUser_ShouldReturnCreated_WhenResponseIsSucessfull()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var user = new CreateUserDto("danapao.va@gmail.com", "Dana", "Vasquez", "Danapao1511");
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Created,
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms")), default))
            .ReturnsAsync(tokenResponse);

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        //Act
        var response = await service.CreateUserAsync(user);
        //Assert
        Assert.Equal(HttpStatusCode.Created, response);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenResponseIsUnsucessful()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var mockKeycloakService = new Mock<IKeycloackService>();
        var service = new KeycloakServices(mockClient.Object);
        var user = new CreateUserDto("danapao.va@gmail.com", "Dana", "Vasquez", "Danapao1511");
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            ResponseStatus = ResponseStatus.Completed,
            Content = "Error en la creación de usuario",
            ContentType = "application/json",
            IsSuccessStatusCode = false
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms")), default))
            .ReturnsAsync(tokenResponse);

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        //Act
        var exception = await Record.ExceptionAsync(() => service.CreateUserAsync(user));
        //Assert
        Assert.NotNull(exception);
        Assert.IsType<HttpRequestException>(exception);
        Assert.Contains("Error en", exception.Message);

    }
    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var user = new CreateUserDto("existinguser@gmail.com", "John", "Doe", "password123");

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Conflict,
            ResponseStatus = ResponseStatus.Completed,
            Content = "Ya existe un usuario con este correo.",
            ContentType = "application/json",
            IsSuccessStatusCode = false
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms")), default))
            .ReturnsAsync(tokenResponse);

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act
        var exception = await Record.ExceptionAsync(() => service.CreateUserAsync(user));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<HttpRequestException>(exception);
        Assert.Contains("Ya existe un usuario", exception.Message);
    }
    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenServerErrorOccurs()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var user = new CreateUserDto("user@gmail.com", "John", "Doe", "password123");

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ResponseStatus = ResponseStatus.Completed,
            Content = "El servidor tuvo un problema interno.",
            ContentType = "application/json",
            IsSuccessStatusCode = false
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/realms")), default))
            .ReturnsAsync(tokenResponse);

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act
        var exception = await Record.ExceptionAsync(() => service.CreateUserAsync(user));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<HttpRequestException>(exception);
        Assert.Contains("El servidor tuvo un problema interno.", exception.Message);
    }
    
    #endregion
    #region GetRolIdAsync
    [Fact]
    public async Task GetRolIdAsync_ExistingRole_ReturnsId()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        string roleName = "admin";
        var expectedRolId = Guid.NewGuid();
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new List<RoleResponse>
            {
                new RoleResponse { id = expectedRolId, name= roleName},
                new RoleResponse { id = Guid.NewGuid(),name="bidder" }
            }),
            ContentType = "application/json",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };


        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains($"/protocol/openid-connect/token")), default))
           .ReturnsAsync(tokenResponse);

        //Act
        var roleId = await service.GetRolIdAsync(roleName);
        //Assert
        Assert.IsType<Guid>(roleId);
        Console.WriteLine($"El roleId obtenido es:{roleId}");
    }
    [Fact]
    public async Task GetRolIdAsync_ShouldThrowException_WhenResponseIsUnsucessful()
    {
        //Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        string roleName = "admin";
        var expectedRolId = Guid.NewGuid();
        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "",
            ContentType = "application/json",
            IsSuccessStatusCode = false
        };
        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };


        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains($"/protocol/openid-connect/token")), default))
           .ReturnsAsync(tokenResponse);

        //Act
        var exception = await Record.ExceptionAsync(() => service.GetRolIdAsync(roleName));
        //Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);

    }
    [Fact]
    public async Task GetRolIdAsync_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var roleName = "admin";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden, // Simulando acceso prohibido
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetRolIdAsync(roleName));
    }
    #endregion
    #region GetCredentialIdAsync
    [Fact]
    public async Task GetCredentialIdAsync_ShouldReturnValidCredentialId_WhenPasswordCredentialExists()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var expectedCredentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new List<CredentialResponse>
            {
                new CredentialResponse { id = expectedCredentialId, type = "password" }
            }),
            ContentType = "application/json",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var credentialId = await service.GetCredentialIdAsync(userId);

        // Assert
        Assert.Equal(expectedCredentialId, credentialId);
    }

    [Fact]
    public async Task GetCredentialIdAsync_ShouldThrowException_WhenRequestFails()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Error en la solicitud"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCredentialIdAsync(userId));
    }

    [Fact]
    public async Task GetCredentialIdAsync_ShouldThrowException_WhenPasswordCredentialNotFound()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new List<CredentialResponse>
            {
                new CredentialResponse { id = Guid.NewGuid().ToString(), type = "totp" } // No es de tipo "password"
            }),
            ContentType = "application/json",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCredentialIdAsync(userId));
    }
    [Fact]
    public async Task GetCredentialIdAsync_ShouldThrowException_WhenResponseIsEmpty()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = null
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCredentialIdAsync(userId));
    }
    #endregion
    #region DeleteCredentialById
    [Fact]
    public async Task DeleteCredentialById_ShouldReturnNoContent_WhenDeletionIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var credentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent // Simula una eliminación exitosa
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var statusCode = await service.DeleteCredentialById(userId, credentialId);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }
    [Fact]
    public async Task DeleteCredentialById_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var credentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = "Acceso prohibido"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteCredentialById(userId, credentialId));
    }
    [Fact]
    public async Task DeleteCredentialById_ShouldThrowException_WhenInternalServerError()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var credentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno del servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteCredentialById(userId, credentialId));
    }

    [Fact]
    public async Task DeleteCredentialById_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var credentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteCredentialById(userId, credentialId));
    }
    [Fact]
    public async Task DeleteCredentialById_ShouldThrowException_WhenCredentialNotFound()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var credentialId = Guid.NewGuid().ToString();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = "Credencial no encontrada"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteCredentialById(userId, credentialId));
    }

    #endregion
    #region ValidCredentials
    [Fact]
    public async Task ValidCredentials_ShouldReturnTrue_WhenCredentialsAreValid()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var username = "validUser";
        var password = "validPassword";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ContentType = "application/json",
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);

        // Act
        var isValid = await service.ValidCredentials(username, password);

        // Assert
        Assert.True(isValid);
    }
    [Fact]
    public async Task ValidCredentials_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var username = "invalidUser";
        var password = "wrongPassword";

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ThrowsAsync(new UnauthorizedAccessException("Error de autenticación"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.ValidCredentials(username, password));
    }


    #endregion
    #region AssingRoleToUserAsync

    [Fact]
    public async Task AssignRoleToUserAsync_ShouldReturnNoContent_WhenAssignmentIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "admin";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent, // Simula una asignación exitosa
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var statusCode = await service.AssingRoleToUserAsync(userId, roleId, roleName);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }
    [Fact]
    public async Task AssignRoleToUserAsync_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "admin";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden, // Simulando acceso prohibido
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AssingRoleToUserAsync(userId, roleId, roleName));
    }
    [Fact]
    public async Task AssignRoleToUserAsync_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "invalid-role";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AssingRoleToUserAsync(userId, roleId, roleName));
    }

    [Fact]
    public async Task AssignRoleToUserAsync_ShouldThrowException_WhenServerErrorOccurs()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleName = "admin";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno en el servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AssingRoleToUserAsync(userId, roleId, roleName));
    }
#endregion
    #region DeleteUserAsync
    [Fact]
    public async Task DeleteUserAsync_ShouldReturnNoContent_WhenDeletionIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var statusCode = await service.DeleteUserAsync(userId);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteUserAsync(userId));
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenInternalServerError()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno en el servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.DeleteUserAsync(userId));
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteUserAsync(userId));
    }
    #endregion
    #region UpdateUserPasswordAsync
    [Fact]
    public async Task UpdateUserPasswordAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var newPassword = "SecurePassword123";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var statusCode = await service.UpdateUserPasswordAsync(userId, newPassword);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var newPassword = "SecurePassword123";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateUserPasswordAsync(userId, newPassword));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ShouldThrowException_WhenInternalServerError()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var newPassword = "SecurePassword123";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno en el servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.UpdateUserPasswordAsync(userId, newPassword));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid().ToString();
        var newPassword = "SecurePassword123";

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserPasswordAsync(userId, newPassword));
    }
    #endregion
    #region SendVerifyEmailToUserAsync
    [Fact]
    public async Task SendVerifyEmailToUserAsync_ShouldReturnNoContent_WhenRequestIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act
        var statusCode = await service.SendVerifyEmailToUserAsync(userId);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    [Fact]
    public async Task SendVerifyEmailToUserAsync_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendVerifyEmailToUserAsync(userId));
    }

    [Fact]
    public async Task SendVerifyEmailToUserAsync_ShouldThrowException_WhenInternalServerError()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno en el servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendVerifyEmailToUserAsync(userId));
    }

    [Fact]
    public async Task SendVerifyEmailToUserAsync_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(), default))
            .ReturnsAsync(mockResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.SendVerifyEmailToUserAsync(userId));
    }
    #endregion

    #region SendEmailForResetPassword
    [Fact]
    public async Task SendEmailForResetPassword_ShouldReturnNoContent_WhenRequestIsSuccessful()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            ResponseStatus = ResponseStatus.Completed,
            IsSuccessStatusCode = true
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act
        var statusCode = await service.SendEmailForResetPassword(userId);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, statusCode);
    }

    [Fact]
    public async Task SendEmailForResetPassword_ShouldThrowException_WhenForbidden()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = "Acceso denegado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendEmailForResetPassword(userId));
    }

    [Fact]
    public async Task SendEmailForResetPassword_ShouldThrowException_WhenInternalServerError()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = "Error interno en el servidor"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendEmailForResetPassword(userId));
    }

    [Fact]
    public async Task SendEmailForResetPassword_ShouldThrowException_WhenBadRequest()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = "Solicitud incorrecta"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.SendEmailForResetPassword(userId));
    }

    [Fact]
    public async Task SendEmailForResetPassword_ShouldThrowException_WhenNotFound()
    {
        // Arrange
        var mockClient = new Mock<IRestClient>();
        var service = new KeycloakServices(mockClient.Object);
        var userId = Guid.NewGuid();

        var mockResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = "El recurso no fue encontrado o está deshabilitado"
        };

        var tokenResponse = new RestResponse
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(new { access_token = "mocked_token" }),
            ResponseStatus = ResponseStatus.Completed,
            ContentType = "application/json",
            IsSuccessStatusCode = true
        };

        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("/protocol/openid-connect/token")), default))
            .ReturnsAsync(tokenResponse);
        mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource.Contains("admin/realms")), default))
            .ReturnsAsync(mockResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendEmailForResetPassword(userId));
    }
    #endregion
}
