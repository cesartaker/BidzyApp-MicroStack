using System;
using System.Collections.Generic;
using Application.Events.InternalEvents;
using System.Net;
using Application.Commands.Handlers;
using Application.Commands;
using Application.DTOs;
using Application.DTOs.MongoDTOs;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Application.Exceptions;
using Application.Contracts.Services;

namespace Usuarios.Tests.Application.Commands.Handlers;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateUserSuccessfully_WhenDataIsValid()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var mockUserService = new Mock<IUserService>();
        var mockValidator = new Mock<IValidator<CreateUserCommand>>();
        var mockKeycloakService = new Mock<IKeycloackService>();

        var handler = new CreateUserCommandHandler(mockMediator.Object, mockUserService.Object, mockValidator.Object, mockKeycloakService.Object);
        var command = new CreateUserCommand("John", "Middle", "Doe", "SecondDoe", "john.doe@gmail.com", "+584129780075", "123 Street", "Admin", "SecurePassword123");

        // Simular validación exitosa
        mockValidator.Setup(v => v.Validate(command)).Returns(new ValidationResult());

        // Simular usuario NO existente
        mockUserService.Setup(s => s.UserExistAsync(command.Email)).ReturnsAsync(false);

        // Simular creación en Keycloak
        mockKeycloakService.Setup(k => k.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(HttpStatusCode.Created);
        mockKeycloakService.Setup(k => k.GetUserIdAsync(command.Email)).ReturnsAsync(Guid.NewGuid());
        mockKeycloakService.Setup(k => k.GetRolIdAsync(command.rolName)).ReturnsAsync(Guid.NewGuid());

        // Simular obtención de rol
        mockUserService.Setup(s => s.GetRoleAsync(command.rolName)).ReturnsAsync(new MongoRoleDto(new Guid().ToString(),new Guid().ToString(),"bidder"));

        // Simular creación de usuario en base de datos
        mockUserService.Setup(s => s.AddUserAsync(It.IsAny<User>())).ReturnsAsync(HttpStatusCode.OK);

        // Simular envío de evento
        mockMediator.Setup(m => m.Publish(It.IsAny<UserCreatedEventNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedUserResponseDto>(result);
        Assert.Equal("John Middle Doe SecondDoe", result.FullName);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<CreateUserCommand>>();
        var handler = new CreateUserCommandHandler(Mock.Of<IMediator>(), Mock.Of<IUserService>(), mockValidator.Object, Mock.Of<IKeycloackService>());
        var command = new CreateUserCommand("", "", "", "", "invalid-email", "", "", "", "");

        // Simular validación fallida
        mockValidator.Setup(v => v.Validate(command)).Returns(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Email", "Formato de email inválido") }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    
}

