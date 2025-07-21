using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories;

public class MongoPaymentMethodWriteRepository : IMongoPaymentMethodWriteRepository
{
    private readonly MongoDbWriteContext _context;
    public MongoPaymentMethodWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Agrega un nuevo método de pago de usuario a la colección <c>UserPaymentMethods</c> en MongoDB.
    /// Utiliza <c>InsertOneAsync</c> para realizar la operación de persistencia de forma asincrónica.
    /// </summary>
    /// <param name="paymentMethod">
    /// Objeto <see cref="UserPaymentMethod"/> que representa el método de pago que será almacenado.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de inserción del método de pago.
    /// </returns>
    public async Task AddPaymentMethod(UserPaymentMethod paymentMethod)
    {
        await _context.UserPaymentMethods.InsertOneAsync(paymentMethod);
    }
}
