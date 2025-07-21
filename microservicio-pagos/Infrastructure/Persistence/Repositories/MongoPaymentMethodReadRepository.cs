using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoPaymentMethodReadRepository : IMongoPaymentMethodReadRepository
{
    private readonly MongoDbReadContext _context;
    public MongoPaymentMethodReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Agrega un nuevo método de pago del usuario al repositorio de lectura almacenado en MongoDB.
    /// Utiliza <c>InsertOneAsync</c> para persistir el objeto <see cref="UserPaymentMethod"/> en la colección correspondiente.
    /// </summary>
    /// <param name="paymentMethod">
    /// Objeto <see cref="UserPaymentMethod"/> que representa el método de pago a almacenar en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de inserción del registro en MongoDB.
    /// </returns>
    public async Task AddPaymentMethod(UserPaymentMethod paymentMethod)
    {
        await _context.UserPaymentMethods.InsertOneAsync(paymentMethod);
    }
    /// <summary>
    /// Recupera todos los métodos de pago registrados para un usuario específico desde MongoDB.
    /// Construye un filtro con <c>Builders.Filter.Eq</c> sobre la propiedad <c>UserId</c>
    /// y ejecuta la búsqueda asincrónica en la colección <c>UserPaymentMethods</c>.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) para filtrar los métodos de pago.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{UserPaymentMethod}}"/> que representa la operación asincrónica de consulta:
    /// retorna una lista de objetos <see cref="UserPaymentMethod"/> correspondientes al usuario especificado,
    /// o una lista vacía si no hay registros coincidentes.
    /// </returns>
    public async Task<List<UserPaymentMethod>> GetPaymentMethodsByUserId(Guid userId)
    {
        var filter = Builders<UserPaymentMethod>.Filter.Eq(upm => upm.UserId, userId);
        var paymentMethods = await _context.UserPaymentMethods.Find(filter).ToListAsync();
        return paymentMethods;

    }
}
