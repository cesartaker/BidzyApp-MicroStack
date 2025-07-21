using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoPaymentReadRepository: IMongoPaymentReadRepository
{
    private readonly MongoDbReadContext _context;
    public MongoPaymentReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Agrega un nuevo registro de pago en la colección <c>Payments</c> de MongoDB.
    /// Utiliza <c>InsertOneAsync</c> para realizar la operación de persistencia de manera asincrónica.
    /// Retorna <see cref="HttpStatusCode.OK"/> si la inserción se realiza exitosamente.
    /// </summary>
    /// <param name="payment">
    /// Objeto <see cref="Payment"/> que contiene los datos del pago que será almacenado en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna <see cref="HttpStatusCode.OK"/> para indicar éxito en la inserción.
    /// </returns>
    public async Task<HttpStatusCode> AddPayment(Payment payment)
    {
        await _context.Payments.InsertOneAsync(payment);
        return HttpStatusCode.OK;
    }
    /// <summary>
    /// Recupera un pago específico desde la colección <c>Payments</c> en MongoDB mediante su identificador único.
    /// Construye un filtro sobre la propiedad <c>Id</c> y ejecuta una consulta asincrónica para obtener el primer resultado coincidente.
    /// </summary>
    /// <param name="paymentId">
    /// Identificador único del pago (<see cref="Guid"/>) que se desea recuperar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Payment}"/> que representa la operación asincrónica:
    /// retorna el objeto <see cref="Payment"/> correspondiente al identificador, o <c>null</c> si no se encuentra.
    /// </returns>
    public async Task<Payment> GetPaymentById(Guid paymentId)
    {
        
        var filter = Builders<Payment>.Filter.Eq(p => p.Id, paymentId);
        var result = await _context.Payments.Find(filter).FirstOrDefaultAsync();
        return result;
    }
    /// <summary>
    /// Recupera todos los pagos con estado <c>PaymentStatus.pending</c> asociados a un usuario específico desde la colección <c>Payments</c> en MongoDB.
    /// Construye un filtro compuesto sobre las propiedades <c>UserId</c> y <c>Status</c>
    /// utilizando <c>Builders.Filter.And</c> para aplicar ambas condiciones.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyas transacciones pendientes se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Payment}}"/> que representa la operación asincrónica de consulta:
    /// retorna una lista de objetos <see cref="Payment"/> que cumplen con ambos criterios de filtrado,
    /// o una lista vacía si no hay coincidencias.
    /// </returns>
    public async Task<List<Payment>> GetPendingPaymentsByUserId(Guid userId)
    {
        var filter = Builders<Payment>.Filter.And(
            Builders<Payment>.Filter.Eq(p => p.UserId, userId),
            Builders<Payment>.Filter.Eq(p => p.Status, PaymentStatus.pending)
        );

        var payments = await _context.Payments.Find(filter).ToListAsync();
        return payments;
    }
    /// <summary>
    /// Actualiza un registro de pago existente en la colección <c>Payments</c> de MongoDB.
    /// Utiliza <c>ReplaceOneAsync</c> para sustituir el documento cuyo <c>Id</c> coincide con el del objeto <see cref="Payment"/> proporcionado.
    /// La respuesta HTTP indica el resultado: <see cref="HttpStatusCode.OK"/> si se modificó exitosamente,
    /// <see cref="HttpStatusCode.NotFound"/> si no hubo coincidencias, o <see cref="HttpStatusCode.NoContent"/> si no se realizaron cambios.
    /// </summary>
    /// <param name="payment">
    /// Objeto <see cref="Payment"/> con los datos actualizados que se desea persistir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica de actualización:
    /// retorna un código HTTP que refleja el resultado de la operación.
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

