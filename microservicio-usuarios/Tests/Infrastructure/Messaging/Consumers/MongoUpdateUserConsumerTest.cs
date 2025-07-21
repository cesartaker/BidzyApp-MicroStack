using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs.MongoDTOs;
using Domain.ValueObjects;
using Infrastructure.Messaging.Cosumers;
using MassTransit;
using Moq;

namespace Usuarios.Tests.Infrastructure.Messaging.Consumers;

public class MongoUpdateUserConsumerTest
{
    [Fact]
    public async Task Consume_ShouldCallUpdateUser_WithCorrectMongoUserDto()
    {
        // Arrange
        var mockRepository = new Mock<IMongoUserRepository>();
        var consumer = new MongoUpdateUserConsumer(mockRepository.Object);
        var userDto = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+589780075"), "123 Street", Guid.NewGuid(), "Admin"
        );

        var mockContext = new Mock<ConsumeContext<MongoUserDto>>();
        mockContext.Setup(c => c.Message).Returns(userDto);

        mockRepository.Setup(r => r.UpdateUser(It.IsAny<MongoUserDto>()))
            .ReturnsAsync(true);

        // Act
        await consumer.Consume(mockContext.Object);

        // Assert
        mockRepository.Verify(r => r.UpdateUser(It.Is<MongoUserDto>(u =>
            u.Id == userDto.Id &&
            u.FirstName == userDto.FirstName &&
            u.MiddleName == userDto.MiddleName &&
            u.LastName == userDto.LastName &&
            u.SecondLastName == userDto.SecondLastName &&
            u.Email == userDto.Email &&
            u.PhoneNumber == userDto.PhoneNumber &&
            u.Address == userDto.Address &&
            u.RoleId == userDto.RoleId &&
            u.RoleName == userDto.RoleName
        )), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogError_WhenUpdateUserFails()
    {
        // Arrange
        var mockRepository = new Mock<IMongoUserRepository>();
        var consumer = new MongoUpdateUserConsumer(mockRepository.Object);
        var userDto = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+589780075"), "123 Street", Guid.NewGuid(), "Admin"
        );

        var mockContext = new Mock<ConsumeContext<MongoUserDto>>();
        mockContext.Setup(c => c.Message).Returns(userDto);

        mockRepository.Setup(r => r.UpdateUser(It.IsAny<MongoUserDto>()))
            .ReturnsAsync(false);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await consumer.Consume(mockContext.Object);

        // Assert
        var output = consoleOutput.ToString();
        Assert.Contains("Error: No se pudo agregar el usuario a MongoDB", output);
    }

    [Fact]
    public async Task Consume_ShouldHandleException_WhenUpdateUserFails()
    {
        // Arrange
        var mockRepository = new Mock<IMongoUserRepository>();
        var consumer = new MongoUpdateUserConsumer(mockRepository.Object);
        var mockContext = new Mock<ConsumeContext<MongoUserDto>>();
        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "Admin"
        );

        // Simular que `context.Message` devuelve un usuario válido
        mockContext.Setup(c => c.Message).Returns(user);

        // Simular que `UpdateUser()` lanza una excepción
        mockRepository.Setup(r => r.UpdateUser(It.IsAny<MongoUserDto>()))
            .ThrowsAsync(new Exception("Simulated failure"));

        // Act
        await consumer.Consume(mockContext.Object);

        // Assert
        mockRepository.Verify(r => r.UpdateUser(It.IsAny<MongoUserDto>()), Times.Once);
    }
}
