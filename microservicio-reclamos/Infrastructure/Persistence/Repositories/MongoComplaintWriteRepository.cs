using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Respositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoComplaintWriteRepository:IMongoComplaintWriteRepository
{
    private readonly MongoDbWriteContext _context;
    private readonly ILogger<MongoComplaintWriteRepository> _logger;

    public MongoComplaintWriteRepository(MongoDbWriteContext context, ILogger<MongoComplaintWriteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    /// Inserta un objeto <see cref="Complaint"/> en la base de datos de escritura.
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
            _logger.LogInformation("Insertando Complaint en la base de datos de escritura: {@Complaint}", complaint);
            await _context.Complaints.InsertOneAsync(complaint);
            _logger.LogInformation("Complaint Insertado en la base de datos de escritura: {@Complaint}", complaint);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar Complaint en la base de datos de escritura: {Mensaje}", ex.Message);
            return HttpStatusCode.InternalServerError;
        }
    }
    /// <summary>
    /// Actualiza la información de un reclamo existente en la base de datos de escritura, 
    /// utilizando su identificador como filtro. Registra el proceso mediante logs
    /// y lanza una excepción si ocurre algún error durante la operación.
    /// </summary>
    /// <param name="complaint">Objeto <see cref="Complaint"/> que contiene los datos actualizados del reclamo.</param>
    /// <exception cref="Exception">
    /// Se lanza cuando ocurre un error al intentar actualizar el reclamo en la base de datos de escritura.
    /// </exception>
    public async Task UpdateComplaint(Complaint complaint)
    {
        try
        {
            if (complaint != null)
            {
                _logger.LogInformation("Actualizando el reclamo en la base de datos de escritura: {@Complaint}", complaint);
                var filter = Builders<Complaint>.Filter.Eq(p => p.Id, complaint.Id);
                var resultado = await _context.Complaints.ReplaceOneAsync(filter, complaint);
                _logger.LogInformation("Reclamo actualizado en la base de datos de escritura: {@Complaint}", complaint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el reclamo: {Mensaje}", ex.Message);
            throw;
        }
    }
}
