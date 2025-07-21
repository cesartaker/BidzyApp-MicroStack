using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Respositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoComplaintReadRepository : IMongoComplaintReadRespository
{
    private readonly MongoDbReadContext _context;
    private readonly ILogger<MongoComplaintReadRepository> _logger;

    public MongoComplaintReadRepository(MongoDbReadContext context, ILogger<MongoComplaintReadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    /// Inserta un objeto <see cref="Complaint"/> en la base de datos de lectura.
    /// Registra el proceso mediante logs y maneja errores devolviendo el estado HTTP correspondiente.
    /// </summary>
    /// <param name="complaint">Objeto <see cref="Complaint"/> que representa el reclamo a insertar.</param>
    /// <returns>
    /// Un código <see cref="HttpStatusCode"/> que indica el resultado de la operación:
    /// <c>OK</c> si fue exitoso, <c>BadRequest</c> si el objeto es nulo,
    /// o <c>InternalServerError</c> en caso de error.
    /// </returns>
    public async Task<HttpStatusCode> AddComplaint(Complaint complaint)
    {
        try
        {
            if (complaint == null)
                return HttpStatusCode.BadRequest;
            _logger.LogInformation("Insertando Complaint en la base de datos de lectura: {@Complaint}", complaint);
            await _context.Complaints.InsertOneAsync(complaint);
            _logger.LogInformation("Complaint Insertado en la base de datos de lectura: {@Complaint}", complaint);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar Complaint en la base de datos de lectura: {Mensaje}", ex.Message);
            return HttpStatusCode.InternalServerError;
        }
    }
    /// <summary>
    /// Recupera un reclamo desde la base de datos de lectura utilizando su identificador único.
    /// Registra el proceso mediante logs y lanza una excepción en caso de error.
    /// </summary>
    /// <param name="complaintId">Identificador único del reclamo que se desea obtener.</param>
    /// <returns>
    /// Un objeto <see cref="Complaint"/> correspondiente al identificador especificado.
    /// Si no se encuentra el reclamo, se devuelve <c>null</c>.
    /// </returns>
    /// <exception cref="Exception">
    /// Se lanza cuando ocurre un error durante el acceso a la base de datos de lectura.
    /// </exception>
    public Complaint GetComplaintById(Guid complaintId)
    {
        var filter = Builders<Complaint>.Filter.Eq(c => c.Id, complaintId);
        try
        {
            _logger.LogInformation("Accediendo a la base de datos de lectura");
            var complaint = _context.Complaints.Find(filter).FirstOrDefault();
            _logger.LogInformation("Información recuperada de forma exitosa de la base de datos de lectura");
            return complaint;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al acceder a la base de datos de lectura: {Mensaje}", ex.Message);
            throw;
        }
    }
    /// <summary>
    /// Recupera un reclamo de la base de datos de lectura utilizando su identificador único.
    /// Registra información del proceso y lanza excepciones en caso de error.
    /// </summary>
    /// <param name="complaintId">Identificador único del reclamo que se desea consultar.</param>
    /// <returns>
    /// Un objeto <see cref="Complaint"/> que representa el reclamo encontrado.
    /// Si no existe el reclamo con el ID especificado, se devuelve <c>null</c>.
    /// </returns>
    /// <exception cref="Exception">
    /// Se lanza cuando ocurre un error al acceder a la base de datos de lectura.
    /// </exception>
    public Task<List<Complaint>> GetComplaintsByStatus(ComplaintStatus status)
    {
        var filter = Builders<Complaint>.Filter.Eq(c => c.Status, status);
        try
        {
            _logger.LogInformation("Accediendo a la base de datos de lectura");
            var complaints = _context.Complaints.Find(filter).ToListAsync();
            _logger.LogInformation("Información recuperada de forma exitosa de la base de datos de lectura");
            return complaints;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al acceder a la base de datos de lectura: {Mensaje}", ex.Message);
            throw;
        }  
    }
    /// <summary>
    /// Recupera todos los reclamos asociados a un usuario específico desde la base de datos de lectura.
    /// Registra el proceso mediante logs y devuelve una lista vacía si no se encuentran resultados.
    /// </summary>
    /// <param name="UserId">Identificador único del usuario cuyos reclamos se desean consultar.</param>
    /// <returns>
    /// Una lista de objetos <see cref="Complaint"/> correspondientes al usuario. 
    /// Si no se encuentran reclamos, se devuelve una lista vacía.
    /// </returns>
    /// <exception cref="Exception">
    /// Se lanza cuando ocurre un error al acceder a la base de datos de lectura.
    /// </exception>
    public Task<List<Complaint>> GetComplaintsByUserId(Guid UserId)
    {
        var fileter = Builders<Complaint>.Filter.Eq(c => c.UserId, UserId);
        try
        {
            _logger.LogInformation("Accediendo a la base de datos de lectura");
            var complaints = _context.Complaints.Find(fileter).ToListAsync();
            if (complaints == null || complaints.Result.Count == 0)
            {
                _logger.LogInformation("No se encontraron reclamos para el usuario con ID: {UserId}", UserId);
                return Task.FromResult(new List<Complaint>());
            }
            _logger.LogInformation("Información recuperada de forma exitosa de la base de datos de lectura");
            return complaints;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al acceder a la base de datos de lectura: {Mensaje}", ex.Message);
            throw;
        }
    }
    /// <summary>
    /// Actualiza un reclamo existente en la base de datos de lectura mediante su identificador.
    /// Registra el proceso en el sistema de logs y maneja posibles excepciones durante la operación.
    /// </summary>
    /// <param name="complaint">Objeto <see cref="Complaint"/> que contiene la información actualizada del reclamo.</param>
    /// <exception cref="Exception">
    /// Se lanza si ocurre un error durante la operación de actualización en la base de datos.
    /// </exception>
    public async Task UpdateComplaint(Complaint complaint)
    {
        try
        {
            if (complaint != null)
            {
                _logger.LogInformation("Actualizando el reclamo en la base de datos de lectura: {@Complaint}", complaint);
                var filter = Builders<Complaint>.Filter.Eq(p => p.Id, complaint.Id);
                var resultado = await _context.Complaints.ReplaceOneAsync(filter, complaint);
                _logger.LogInformation("Reclamo actualizado en la base de datos de lectura: {@Complaint}", complaint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el reclamo: {Mensaje}", ex.Message);
            throw;
        }  
    }
}
