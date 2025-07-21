using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

public class MongoWaybillWriteRepository : IMongoWaybillWriteRepository
{
    private readonly MongoDbWriteContext _context;

    public MongoWaybillWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta una nueva guía de entrega (<see cref="Waybill"/>) en el repositorio MongoDB.
    /// Valida que el objeto recibido no sea nulo antes de persistirlo.
    /// Si la operación se realiza con éxito, devuelve <see cref="HttpStatusCode.OK"/>.
    /// Si el objeto es nulo o ocurre una excepción durante el proceso, se retorna el código HTTP correspondiente.
    /// </summary>
    /// <param name="waybill">
    /// Objeto <see cref="Waybill"/> que contiene los datos de la guía de entrega a registrar,
    /// incluyendo subasta asociada, datos del receptor, dirección y detalles logísticos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// <c>OK</c> si la guía se registra correctamente,
    /// <c>BadRequest</c> si el objeto es nulo,
    /// o <c>InternalServerError</c> si ocurre una excepción inesperada.
    /// </returns>
    public async Task<HttpStatusCode> AddWaybill(Waybill waybill)
    {
        try
        {
            if (waybill == null)
                return HttpStatusCode.BadRequest;

            await _context.Waybills.InsertOneAsync(waybill);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al insertar producto: {ex.Message}");
            return HttpStatusCode.InternalServerError;
        }
    }
}
