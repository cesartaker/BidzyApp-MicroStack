using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Usuarios.Tests.Infrastructure.Persistence;

public class UnitOfWorkTest
{
    private readonly DbContextOptions<AppDbContext> _options;

    public UnitOfWorkTest()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }


    [Fact]
    public async Task BeginTransactionAsync_ShouldInvokeBeginTransaction()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        // Configurar `Database` para devolver el `DatabaseFacade` mockeado
        mockContext.SetupGet(c => c.Database).Returns(mockDatabaseFacade.Object);

        // Simular la ejecución de `BeginTransactionAsync()` devolviendo una transacción mockeada
        mockDatabaseFacade.Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        var unitOfWork = new UnitOfWork(mockContext.Object);

        // Act
        await unitOfWork.BeginTransactionAsync();

        // Assert
        mockDatabaseFacade.Verify(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CommitAsync_ShouldCommitTransaction_WhenChangesAreMade()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        // Configurar `Database` para devolver el `DatabaseFacade` mockeado
        mockContext.SetupGet(c => c.Database).Returns(mockDatabaseFacade.Object);

        // Simular que `SaveChangesAsync()` devuelve un resultado mayor a 0 (indicando cambios)
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Simular que hay una transacción activa
        mockDatabaseFacade.SetupGet(db => db.CurrentTransaction).Returns(mockTransaction.Object);

        // Simular la ejecución de `CommitTransactionAsync()`
        mockDatabaseFacade.Setup(db => db.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var unitOfWork = new UnitOfWork(mockContext.Object);

        // Act
        var result = await unitOfWork.CommitAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result);
        mockDatabaseFacade.Verify(db => db.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CommitAsync_ShouldRollbackTransaction_WhenNoChanges()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        // Configurar `Database` para devolver el `DatabaseFacade` mockeado
        mockContext.SetupGet(c => c.Database).Returns(mockDatabaseFacade.Object);

        // Simular que `SaveChangesAsync()` devuelve `0` (sin cambios)
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        // Simular que hay una transacción activa
        mockDatabaseFacade.SetupGet(db => db.CurrentTransaction).Returns(mockTransaction.Object);

        // Simular la ejecución de `RollbackTransactionAsync()`
        mockDatabaseFacade.Setup(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var unitOfWork = new UnitOfWork(mockContext.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => unitOfWork.CommitAsync()); // Validamos que lanza una excepción
        mockDatabaseFacade.Verify(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce); // ✅ Permitir más de una ejecución
    }
    [Fact]
    public async Task CommitAsync_ShouldRollbackTransaction_WhenSaveChangesFails()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        // Configurar `Database` para devolver el `DatabaseFacade` mockeado
        mockContext.SetupGet(c => c.Database).Returns(mockDatabaseFacade.Object);

        // Simular que `SaveChangesAsync()` lanza una excepción
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Simulated failure"));

        // Simular que hay una transacción activa
        mockDatabaseFacade.SetupGet(db => db.CurrentTransaction).Returns(mockTransaction.Object);

        // Simular la ejecución de `RollbackTransactionAsync()`
        mockDatabaseFacade.Setup(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var unitOfWork = new UnitOfWork(mockContext.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => unitOfWork.CommitAsync()); // Validamos que lanza la excepción esperada
        mockDatabaseFacade.Verify(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
    [Fact]
    public async Task RollbackAsync_ShouldRollbackTransaction()
    {
        // Arrange
        var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        // Configurar `Database` para devolver el `DatabaseFacade` mockeado
        mockContext.SetupGet(c => c.Database).Returns(mockDatabaseFacade.Object);

        // Simular que hay una transacción activa
        mockDatabaseFacade.SetupGet(db => db.CurrentTransaction).Returns(mockTransaction.Object);

        // Simular la ejecución de `RollbackTransactionAsync()`
        mockDatabaseFacade.Setup(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWork = new UnitOfWork(mockContext.Object);

        // Act
        await unitOfWork.RollbackAsync();

        // Assert
        mockDatabaseFacade.Verify(db => db.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
