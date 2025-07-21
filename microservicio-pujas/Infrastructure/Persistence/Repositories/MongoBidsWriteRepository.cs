using System.Net;
using Domain.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories;

public class MongoBidsWriteRepository : IMongoBidsWriteRepository
{
    private readonly MongoDbWriteContext _context;
    public MongoBidsWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta una nueva puja (<see cref="Bid"/>) en la colección de MongoDB.
    /// Verifica si el objeto es nulo y retorna <see cref="HttpStatusCode.BadRequest"/> en ese caso.
    /// Si la inserción es exitosa mediante <c>InsertOneAsync</c>, retorna <see cref="HttpStatusCode.OK"/>.
    /// Si ocurre una excepción durante la operación, la captura, muestra el mensaje de error y retorna <see cref="HttpStatusCode.InternalServerError"/>.
    /// </summary>
    /// <param name="bid">
    /// Objeto <see cref="Bid"/> que representa la puja a insertar en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// el código de estado HTTP indica el resultado de la operación.
    /// </returns>
    public async Task<HttpStatusCode> AddBidAsync(Bid bid)
    {
        try
        {
            if (bid == null)
                return HttpStatusCode.BadRequest;

            await _context.Bids.InsertOneAsync(bid);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al insertar producto: {ex.Message}");
            return HttpStatusCode.InternalServerError;
        }
    }
}
