using System.Security.Authentication;
using FluentValidation;
using Application.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Usuarios.API.Controllers;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Usuarios.API.Requests;

namespace Usuarios.Tests.Presentation.Controllers;
public class UserControllersTest 
{
    [Fact]
    public async Task CreateUser_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        var expectedResponse = new CreatedUserResponseDto(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "Admin", "Usuario creado exitosamente"
        );

        // Simular que `Send()` retorna la respuesta esperada
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.CreateUser(command);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

    }
    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenValidationExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        // Simular que `Send()` lanza una `ValidationException`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Error de validación\nCampo inválido"));

        // Act
        var result = await controller.CreateUser(command) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
    [Fact]
    public async Task CreateUser_ShouldReturnConflict_WhenArgumentExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        // Simular que `Send()` lanza una `ArgumentException`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("El usuario ya existe"));

        // Act
        var result = await controller.CreateUser(command) as ConflictObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnUnauthorized_WhenAuthenticationExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        // Simular que `Send()` lanza una `AuthenticationException`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException("Usuario no autorizado"));

        // Act
        var result = await controller.CreateUser(command) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnServiceUnavailable_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        // Simular que `Send()` lanza una `HttpRequestException`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Error al comunicarse con el servidor"));

        // Act
        var result = await controller.CreateUser(command) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(503, result.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnInternalServerError_WhenInvalidOperationExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Operación inválida en el sistema"));

        // Act
        var result = await controller.CreateUser(command) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new CreateUserCommand(
            "John", "Middle", "Doe", "SecondDoe",
            "john.doe@gmail.com", "+584129780075", "123 Street",
            "Admin", "SecurePassword123"
        );

        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error inesperado en el sistema"));

        // Act
        var result = await controller.CreateUser(command) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new UpdateUserRequest(
            "John", "Middle", "Doe", "SecondDoe",
            "+584129780075", "123 Street"
        );

        
        var token = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3T2NqTTVLZlZZVHExbG9uMHZBRzYyUU5OT3IxOWNNNFlUVFJjVVBVM2I4In0.eyJleHAiOjE3NDg1NzIzNzEsImlhdCI6MTc0ODU3MDU3MSwiYXV0aF90aW1lIjoxNzQ4NTcwNTY2LCJqdGkiOiJvbnJ0YWM6YWYwNDkzNDUtMWY4NS00N2ZlLTg2NjEtNTQxM2NlNjg0OTNjIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy9iaWR6eS1hcHAiLCJhdWQiOlsiY29uZmlkZW50aWFsLWNsaWVudCIsImFjY291bnQiXSwic3ViIjoiZTI4ZDViOWYtODc3Zi00MTJkLTgwMDUtMTc2Yzg1ODBkOTlmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicHVibGljLWNsaWVudCIsInNpZCI6ImYzNjE0MDZhLTY1NTMtNDIxMi1iM2QwLWI3N2JkODEzOTAyNCIsImFjciI6IjAiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo0MDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJiaWRkZXIiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy1iaWR6eS1hcHAiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImF1ZGllbmNlIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkNlc2FyIEJhdXRlIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiY2piYXV0ZS4xOUBnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiQ2VzYXIiLCJmYW1pbHlfbmFtZSI6IkJhdXRlIiwiZW1haWwiOiJjamJhdXRlLjE5QGdtYWlsLmNvbSJ9.d735MzDkX5tINdIWz-X-7KmRammYghCYJGzT-3pQEDbLt076uVueu7moChw4Hj66rJ80X4q65fO478-0Ldme9oNDhX77RBlK0iDmirCjXfc00a1IC8WMiavGpKuHm2vLsvLXoAj0rUcZb6jYGTEJaRLda6DYe0WzW-qIx-fSv-G7ktWR0EYVhvffA7YQrWLBbj_JHGETcuVFn1s-bPL4GHVASINfkKP9HqFTowaGbryYveESmexJB77kWwSN5cBsD89PvPtYv1wPhq9xZqfr0w4_tnSYDoswQwpclipqmEiOeOe1ERt1TZY23vgJTtArwoWjZpW98D6nBQhL5POgGQ";
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = token;

        mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdatedUserResponseDto("", "", "", "", "", ""));

        // Act
        var result = await controller.UpdateUser(command);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUnauthorized_WhenTokenMissingUserIdOrEmail()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new UpdateUserRequest(
            "John", "Middle", "Doe", "SecondDoe",
            "+584129780075", "123 Street"
        );

        // Simular un token sin `sub` ni `email`
        var invalidToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = invalidToken;

        // Act
        var result = await controller.UpdateUser(command) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new UpdateUserRequest(
            "John", "Middle", "Doe", "SecondDoe",
            "+584129780075", "123 Street"
        );

        var token = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3T2NqTTVLZlZZVHExbG9uMHZBRzYyUU5OT3IxOWNNNFlUVFJjVVBVM2I4In0.eyJleHAiOjE3NDg1NzIzNzEsImlhdCI6MTc0ODU3MDU3MSwiYXV0aF90aW1lIjoxNzQ4NTcwNTY2LCJqdGkiOiJvbnJ0YWM6YWYwNDkzNDUtMWY4NS00N2ZlLTg2NjEtNTQxM2NlNjg0OTNjIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy9iaWR6eS1hcHAiLCJhdWQiOlsiY29uZmlkZW50aWFsLWNsaWVudCIsImFjY291bnQiXSwic3ViIjoiZTI4ZDViOWYtODc3Zi00MTJkLTgwMDUtMTc2Yzg1ODBkOTlmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicHVibGljLWNsaWVudCIsInNpZCI6ImYzNjE0MDZhLTY1NTMtNDIxMi1iM2QwLWI3N2JkODEzOTAyNCIsImFjciI6IjAiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo0MDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJiaWRkZXIiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy1iaWR6eS1hcHAiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImF1ZGllbmNlIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkNlc2FyIEJhdXRlIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiY2piYXV0ZS4xOUBnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiQ2VzYXIiLCJmYW1pbHlfbmFtZSI6IkJhdXRlIiwiZW1haWwiOiJjamJhdXRlLjE5QGdtYWlsLmNvbSJ9.d735MzDkX5tINdIWz-X-7KmRammYghCYJGzT-3pQEDbLt076uVueu7moChw4Hj66rJ80X4q65fO478-0Ldme9oNDhX77RBlK0iDmirCjXfc00a1IC8WMiavGpKuHm2vLsvLXoAj0rUcZb6jYGTEJaRLda6DYe0WzW-qIx-fSv-G7ktWR0EYVhvffA7YQrWLBbj_JHGETcuVFn1s-bPL4GHVASINfkKP9HqFTowaGbryYveESmexJB77kWwSN5cBsD89PvPtYv1wPhq9xZqfr0w4_tnSYDoswQwpclipqmEiOeOe1ERt1TZY23vgJTtArwoWjZpW98D6nBQhL5POgGQ";
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = token;

        mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => controller.UpdateUser(command));
    }

    [Fact]
    public async Task ResetUserPassword_ShouldReturnOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new ResetUserPasswordCommand("john.doe@gmail.com");

        // Simular que `Send()` retorna `HttpStatusCode.OK`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpStatusCode.OK);

        // Act
        var result = await controller.ResetUserPassword(command) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ResetUserPassword_ShouldReturnBadRequest_WhenValidationExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new ResetUserPasswordCommand("invalid-email");

        // Simular que `Send()` lanza una `ValidationException`
        mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("El email no es válido"));

        // Act
        var result = await controller.ResetUserPassword(command) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task ResetUserPassword_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var command = new ResetUserPasswordCommand("john.doe@gmail.com");

        mockMediator.Setup(m => m.Send(It.IsAny<ResetUserPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error inesperado en el sistema"));

        // Act
        var result = await controller.ResetUserPassword(command) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task UpdateUserPassword_ShouldReturnOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var request = new UpdateUserPasswordRequest("OldPassword123", "NewPassword456");

        // Simular un token JWT válido en los encabezados de la solicitud
        var token = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3T2NqTTVLZlZZVHExbG9uMHZBRzYyUU5OT3IxOWNNNFlUVFJjVVBVM2I4In0.eyJleHAiOjE3NDg1NzIzNzEsImlhdCI6MTc0ODU3MDU3MSwiYXV0aF90aW1lIjoxNzQ4NTcwNTY2LCJqdGkiOiJvbnJ0YWM6YWYwNDkzNDUtMWY4NS00N2ZlLTg2NjEtNTQxM2NlNjg0OTNjIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy9iaWR6eS1hcHAiLCJhdWQiOlsiY29uZmlkZW50aWFsLWNsaWVudCIsImFjY291bnQiXSwic3ViIjoiZTI4ZDViOWYtODc3Zi00MTJkLTgwMDUtMTc2Yzg1ODBkOTlmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicHVibGljLWNsaWVudCIsInNpZCI6ImYzNjE0MDZhLTY1NTMtNDIxMi1iM2QwLWI3N2JkODEzOTAyNCIsImFjciI6IjAiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo0MDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJiaWRkZXIiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy1iaWR6eS1hcHAiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImF1ZGllbmNlIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkNlc2FyIEJhdXRlIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiY2piYXV0ZS4xOUBnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiQ2VzYXIiLCJmYW1pbHlfbmFtZSI6IkJhdXRlIiwiZW1haWwiOiJjamJhdXRlLjE5QGdtYWlsLmNvbSJ9.d735MzDkX5tINdIWz-X-7KmRammYghCYJGzT-3pQEDbLt076uVueu7moChw4Hj66rJ80X4q65fO478-0Ldme9oNDhX77RBlK0iDmirCjXfc00a1IC8WMiavGpKuHm2vLsvLXoAj0rUcZb6jYGTEJaRLda6DYe0WzW-qIx-fSv-G7ktWR0EYVhvffA7YQrWLBbj_JHGETcuVFn1s-bPL4GHVASINfkKP9HqFTowaGbryYveESmexJB77kWwSN5cBsD89PvPtYv1wPhq9xZqfr0w4_tnSYDoswQwpclipqmEiOeOe1ERt1TZY23vgJTtArwoWjZpW98D6nBQhL5POgGQ";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = token;

        // Simular que `Send()` devuelve `HttpStatusCode.OK`
        mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpStatusCode.OK);

        // Act
        var result = await controller.UpdateUserPassword(request);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUserPassword_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var request = new UpdateUserPasswordRequest("OldPassword123", "NewPassword456");

        // Simular un token inválido
        var invalidToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = invalidToken;

        // Act
        var result = await controller.UpdateUserPassword(request) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task UpdateUserPassword_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var controller = new UsersController(mockMediator.Object);
        var request = new UpdateUserPasswordRequest("OldPassword123", "NewPassword456");

        // Simular que `HttpContext.Request.Headers` contiene un token válido
        var token = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3T2NqTTVLZlZZVHExbG9uMHZBRzYyUU5OT3IxOWNNNFlUVFJjVVBVM2I4In0.eyJleHAiOjE3NDg1NzIzNzEsImlhdCI6MTc0ODU3MDU3MSwiYXV0aF90aW1lIjoxNzQ4NTcwNTY2LCJqdGkiOiJvbnJ0YWM6YWYwNDkzNDUtMWY4NS00N2ZlLTg2NjEtNTQxM2NlNjg0OTNjIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy9iaWR6eS1hcHAiLCJhdWQiOlsiY29uZmlkZW50aWFsLWNsaWVudCIsImFjY291bnQiXSwic3ViIjoiZTI4ZDViOWYtODc3Zi00MTJkLTgwMDUtMTc2Yzg1ODBkOTlmIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicHVibGljLWNsaWVudCIsInNpZCI6ImYzNjE0MDZhLTY1NTMtNDIxMi1iM2QwLWI3N2JkODEzOTAyNCIsImFjciI6IjAiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo0MDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJiaWRkZXIiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy1iaWR6eS1hcHAiXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImF1ZGllbmNlIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkNlc2FyIEJhdXRlIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiY2piYXV0ZS4xOUBnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiQ2VzYXIiLCJmYW1pbHlfbmFtZSI6IkJhdXRlIiwiZW1haWwiOiJjamJhdXRlLjE5QGdtYWlsLmNvbSJ9.d735MzDkX5tINdIWz-X-7KmRammYghCYJGzT-3pQEDbLt076uVueu7moChw4Hj66rJ80X4q65fO478-0Ldme9oNDhX77RBlK0iDmirCjXfc00a1IC8WMiavGpKuHm2vLsvLXoAj0rUcZb6jYGTEJaRLda6DYe0WzW-qIx-fSv-G7ktWR0EYVhvffA7YQrWLBbj_JHGETcuVFn1s-bPL4GHVASINfkKP9HqFTowaGbryYveESmexJB77kWwSN5cBsD89PvPtYv1wPhq9xZqfr0w4_tnSYDoswQwpclipqmEiOeOe1ERt1TZY23vgJTtArwoWjZpW98D6nBQhL5POgGQ";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["Authorization"] = token;

        // Simular que `Send()` lanza una excepción inesperada
        mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error inesperado al actualizar la contraseña"));

        // Act
        var result = await controller.UpdateUserPassword(request) as BadRequestResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

}

