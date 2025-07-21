using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.MongoDTOs;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories.WriteRepositories;

public class PostgreUserRepository: IPostgreUserRepository
{
    private readonly AppDbContext _context;

    public PostgreUserRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    /// <summary>
    /// Agrega un nuevo usuario al conjunto de entidades <c>Users</c> del contexto de base de datos PostgreSQL.
    /// No confirma los cambios hasta que se realice una operación de guardado o commit.
    /// </summary>
    /// <param name="user">Objeto <see cref="User"/> que contiene la información del usuario a agregar.</param>
    public void AddUser(User user)
    {
         _context.Users.Add(user);
    }
    /// <summary>
    /// Marca un objeto <see cref="User"/> como modificado en el conjunto <c>Users</c> del contexto de base de datos PostgreSQL.
    /// Los cambios no se guardan hasta que se realice una operación de commit o <c>SaveChanges()</c>.
    /// </summary>
    /// <param name="user">Entidad <see cref="User"/> que contiene los datos actualizados del usuario.</param>
    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
    }
}
