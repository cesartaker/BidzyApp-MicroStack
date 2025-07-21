using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Exceptions;
using Domain.Entities;

namespace Application.Services;

public class UserAuditService : IUserAuditService
{
    public readonly IPostgreUserActivityHistoryRepository _postgreRepository;
    public readonly IUnitOfWork _unitOfWork;

    public UserAuditService(IPostgreUserActivityHistoryRepository repo, IUnitOfWork unitOfWork)
    {
        _postgreRepository = repo;
        _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// Agrega una entrada al historial de actividad de un usuario en la base de datos PostgreSQL.
    /// Inicia una transacción, guarda la actividad y confirma los cambios. Si ocurre un error,
    /// revierte la transacción y registra el mensaje en la consola.
    /// </summary>
    /// <param name="userId">Identificador único del usuario asociado a la actividad.</param>
    /// <param name="description">Descripción de la actividad realizada por el usuario.</param>
    public async Task AddToHistory(Guid userId,string description)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            UserActivityHistory activity = new UserActivityHistory(userId, description);
            _postgreRepository.Add(activity);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            Console.Write($"Error: No se pudo actualizar el historial del usuario: {ex.Message}");
        }
        
    }
    /// <summary>
    /// Recupera el historial de actividad de un usuario a partir de su identificador único.
    /// Consulta la base de datos PostgreSQL mediante el repositorio correspondiente.
    /// </summary>
    /// <param name="id">Identificador único del usuario cuyos registros de actividad se desean obtener.</param>
    /// <returns>
    /// Una lista de objetos <see cref="UserActivityHistory"/> que representan las acciones registradas del usuario.
    /// </returns>
    public async Task<List<UserActivityHistory>> GetHistory(Guid id)
    {
        var history = await _postgreRepository.GetByUserIdAsync(id);
        return history;
    }
}
