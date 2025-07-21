using System.Net;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoPaymentWriteRepository : IMongoPaymentWriteRepository
{
    private readonly MongoDbWriteContext _context;
    public MongoPaymentWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta un nuevo registro de pago en la colección <c>Payments</c> en MongoDB.
    /// Utiliza <c>InsertOneAsync</c> para almacenar el objeto <see cref="Payment"/> de forma asincrónica.
    /// Siempre retorna <see cref="HttpStatusCode.OK"/> como indicador de éxito en la operación.
    /// </summary>
    /// <param name="payment">
    /// Objeto <see cref="Payment"/> que contiene los datos de la transacción a registrar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica de inserción:
    /// retorna <see cref="HttpStatusCode.OK"/> como resultado exitoso.
    /// </returns>
    public async Task<HttpStatusCode> AddPayment(Payment payment)
    {
        await _context.Payments.InsertOneAsync(payment);
        return HttpStatusCode.OK;
    }
    /// <summary>
    /// Actualiza un registro existente en la colección <c>Payments</c> de MongoDB utilizando el identificador <c>Id</c>.
    /// Utiliza <c>ReplaceOneAsync</c> para reemplazar el documento por completo. Retorna un <see cref="HttpStatusCode"/> 
    /// que indica el resultado de la operación: <c>OK</c> si se modificó exitosamente, <c>NotFound</c> si no se encontró 
    /// el documento, o <c>NoContent</c> si no hubo modificaciones.
    /// </summary>
    /// <param name="payment">
    /// Objeto <see cref="Payment"/> que contiene los datos actualizados que se desea persistir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna un código HTTP en función del resultado de la operación de reemplazo.
    /// </returns>
    public async Task<HttpStatusCode> UpdatePayment(Payment payment)
    {
        
        var filter = Builders<Payment>.Filter.Eq(p => p.Id, payment.Id);
        var result = await _context.Payments.ReplaceOneAsync(filter, payment);

        if (result.IsAcknowledged && result.ModifiedCount > 0)
            return HttpStatusCode.OK;

        return result.MatchedCount == 0 ? HttpStatusCode.NotFound : HttpStatusCode.NoContent;
    }

}
