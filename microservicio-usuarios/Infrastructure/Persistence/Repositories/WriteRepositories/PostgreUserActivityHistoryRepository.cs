using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.WriteRepositories;

public class PostgreUserActivityHistoryRepository:IPostgreUserActivityHistoryRepository
{
    private readonly AppDbContext _context;

    public PostgreUserActivityHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Agrega una nueva entrada al conjunto <c>UserActivityHistories</c> del contexto de base de datos PostgreSQL.
    /// La operación no confirma los cambios hasta que se ejecute una transacción o <c>SaveChanges()</c>.
    /// </summary>
    /// <param name="activity">Objeto <see cref="UserActivityHistory"/> que contiene la información de la actividad del usuario.</param>
    public void Add(UserActivityHistory activity)
    {
        _context.UserActivityHistories.Add(activity);
    }
    /// <summary>
    /// Agrega una nueva entrada al conjunto <c>UserActivityHistories</c> del contexto de base de datos PostgreSQL.
    /// La operación no confirma los cambios hasta que se ejecute una transacción o <c>SaveChanges()</c>.
    /// </summary>
    /// <param name="activity">Objeto <see cref="UserActivityHistory"/> que contiene la información de la actividad del usuario.</param>
    public async Task<List<UserActivityHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserActivityHistories
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }


}
