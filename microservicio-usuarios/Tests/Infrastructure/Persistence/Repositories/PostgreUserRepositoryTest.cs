using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.WriteRepositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Usuarios.Tests.Infrastructure.Persistence.Repositories;

public class PostgreUserRepositoryTest
{
    [Fact]
    public void AddUser_ShouldAddUserToDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new AppDbContext(options);
        var repository = new PostgreUserRepository(context);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            MiddleName = "Middle",
            LastName = "Doe",
            SecondLastName = "SecondDoe",
            Email = new Email("john.doe@gmail.com"),
            PhoneNumber = new PhoneNumber("+584121478956"),
            Address = "123 Street",
            RoleId = Guid.NewGuid()
        };

        // Act
        repository.AddUser(user);
        context.SaveChanges();

        // Assert
        var addedUser = context.Users.Find(user.Id);
        Assert.NotNull(addedUser);
        Assert.Equal(user.Email.Value, addedUser.Email.Value);
    }

    [Fact]
    public void UpdateUser_ShouldUpdateUserInDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new AppDbContext(options);
        var repository = new PostgreUserRepository(context);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            MiddleName = "Middle",
            LastName = "Doe",
            SecondLastName = "SecondDoe",
            Email = new Email("john.doe@gmail.com"),
            PhoneNumber = new PhoneNumber("+584129833871"),
            Address = "123 Street",
            RoleId = Guid.NewGuid()
        };

        context.Users.Add(user);
        context.SaveChanges();

        // Act
        user.FirstName = "UpdatedName";
        repository.UpdateUser(user);
        context.SaveChanges();

        // Assert
        var updatedUser = context.Users.Find(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("UpdatedName", updatedUser.FirstName);
    }
}
