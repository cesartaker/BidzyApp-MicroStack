using Application.Exceptions;
using Application.DTOs.MongoDTOs;
using Domain.ValueObjects;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.ReadRepositories;
using MongoDB.Driver;
using Moq;

namespace Usuarios.Tests.Infrastructure.Persistence.Repositories;

public class MongoUserRepositoryTest
{
    [Fact]
    public async Task AddUser_ShouldInsertUser_WhenValid()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);
        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);
        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "Admin"
        );

        mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<MongoUserDto>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        // Act
        var result = await repository.AddUser(user);
        // Assert
        Assert.True(result);
        mockCollection.Verify(c => c.InsertOneAsync(It.Is<MongoUserDto>(u =>
                u.Id == user.Id && u.Email == user.Email),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddUser_ShouldReturnFalse_WhenMongoFails()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "admin"
        );

        // Simular que MongoDB falla al ejecutar `InsertOneAsync`
        mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<MongoUserDto>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Simulated MongoDB failure"));

        // Act
        var result = await repository.AddUser(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUser_ShouldReplaceUser_WhenValid()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "Admin"
        );

        var mockResult = new Mock<ReplaceOneResult>();
        mockResult.Setup(r => r.IsAcknowledged).Returns(true);

        mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MongoUserDto>>(),
                It.IsAny<MongoUserDto>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await repository.UpdateUser(user);

        // Assert
        Assert.True(result);
        mockCollection.Verify(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<MongoUserDto>>(),
            It.Is<MongoUserDto>(u => u.Id == user.Id && u.Email == user.Email),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowTransactionFailureException_WhenMongoFails()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "Admin"
        );

        // Simular que MongoDB falla al ejecutar `ReplaceOneAsync`
        mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MongoUserDto>>(),
                It.IsAny<MongoUserDto>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Simulated MongoDB failure"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TransactionFailureException>(() => repository.UpdateUser(user));
        Assert.Contains("Error al actualizar datos del usuario en MongoDB", exception.Message);
    }

    [Fact]
    public async Task GetRole_ShouldReturnRole_WhenExists()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoRoleDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoRoleDto>("Roles", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var role = new MongoRoleDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "admin");

        var mockCursor = new Mock<IAsyncCursor<MongoRoleDto>>();
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        mockCursor.Setup(c => c.Current).Returns(new List<MongoRoleDto> { role });

        mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoRoleDto>>(),
                It.IsAny<FindOptions<MongoRoleDto>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await repository.GetRole("admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("admin", result.Name);
    }

    [Fact]
    public async Task GetRole_ShouldThrowException_WhenMongoFails()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoRoleDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoRoleDto>("Roles", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoRoleDto>>(),
                It.IsAny<FindOptions<MongoRoleDto>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Simulated MongoDB failure")); // Simula el error

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => repository.GetRole("Admin"));
        Assert.Contains("Error al obtener el rol", exception.Message);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var user = new MongoUserDto(
            Guid.NewGuid(), "John", "Middle", "Doe", "SecondDoe",
            new Email("john.doe@gmail.com"), new PhoneNumber("+584129780075"), "123 Street",
            Guid.NewGuid(), "Admin"
        );

        var mockCursor = new Mock<IAsyncCursor<MongoUserDto>>();
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        mockCursor.Setup(c => c.Current).Returns(new List<MongoUserDto> { user });

        mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoUserDto>>(),
                It.IsAny<FindOptions<MongoUserDto>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await repository.GetUserByEmail("john.doe@gmail.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.Email.Value, result.Email.Value);
    }
    [Fact]
    public async Task GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<MongoUserDto>>();

        mockDatabase.Setup(db => db.GetCollection<MongoUserDto>("Users", null))
            .Returns(mockCollection.Object);

        var mockContext = new MongoDbContext(mockDatabase.Object);
        var repository = new MongoUserRepository(mockContext);

        var mockCursor = new Mock<IAsyncCursor<MongoUserDto>>();
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Simula que no hay resultados en la base de datos

        mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoUserDto>>(),
                It.IsAny<FindOptions<MongoUserDto>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await repository.GetUserByEmail("nonexistent@gmail.com");

        // Assert
        Assert.Null(result); 
    }
}
