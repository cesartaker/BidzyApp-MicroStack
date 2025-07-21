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

public class MongoWaybillReadRepository: IMongoWaybillReadRepository
{
    private readonly MongoDbReadContext _context;

    public MongoWaybillReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta una nueva guía de entrega (<see cref="Waybill"/>) en el repositorio MongoDB.
    /// Valida que el objeto recibido no sea nulo antes de intentar la persistencia. 
    /// En caso de éxito, devuelve <see cref="HttpStatusCode.OK"/>; si la entrada es inválida o ocurre una excepción, devuelve el código correspondiente.
    /// </summary>
    /// <param name="waybill">
    /// Objeto <see cref="Waybill"/> que contiene los datos de la guía de entrega a registrar:
    /// subasta asociada, datos del receptor, dirección, estado de envío, etc.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve <c>OK</c> si la guía se registra correctamente,
    /// <c>BadRequest</c> si el objeto es nulo,
    /// o <c>InternalServerError</c> si ocurre una excepción durante la operación.
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
