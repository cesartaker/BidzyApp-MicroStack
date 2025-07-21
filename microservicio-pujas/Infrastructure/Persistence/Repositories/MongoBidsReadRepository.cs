using System.Net;
using Domain.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoBidsReadRepository : IMongoBidsReadRepository
{
    private readonly MongoDbReadContext _context;

    public MongoBidsReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Recupera la puja más alta (<see cref="Bid"/>) asociada a una subasta específica desde la base de datos MongoDB.
    /// Aplica un filtro por identificador de subasta, ordena por monto descendente y retorna el primer resultado encontrado.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) cuya puja más alta se desea obtener.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Bid}"/> que representa la operación asincrónica de lectura desde MongoDB:
    /// retorna el objeto <see cref="Bid"/> con el mayor monto registrado o <c>null</c> si no se encuentran pujas.
    /// </returns>
    public async Task<Bid> GetHighestBidByAuctionIdAsync(Guid auctionId)
    {
        var filter = Builders<Bid>.Filter.Eq(b => b.AuctionId, auctionId);
        var sort = Builders<Bid>.Sort.Descending(b => b.Amount);

        return await _context.Bids.Find(filter).Sort(sort).Limit(1)
            .FirstOrDefaultAsync();
    }
    /// <summary>
    /// Inserta una nueva puja (<see cref="Bid"/>) en la colección de MongoDB.
    /// Verifica si el objeto es nulo, y en tal caso retorna un <c>BadRequest</c>.
    /// Si la inserción es exitosa, retorna <c>OK</c>; de lo contrario, captura la excepción y retorna <c>InternalServerError</c>.
    /// </summary>
    /// <param name="bid">
    /// Objeto <see cref="Bid"/> que representa la puja a registrar en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna el código de estado HTTP correspondiente al resultado de la operación.
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
    /// <summary>
    /// Recupera todas las pujas (<see cref="Bid"/>) asociadas a una subasta específica desde MongoDB.
    /// Aplica un filtro por <paramref name="auctionId"/> para obtener las ofertas correspondientes.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica:
    /// retorna una lista de objetos <see cref="Bid"/> relacionados con la subasta o una lista vacía si no se encuentran resultados.
    /// </returns>
    public async Task<List<Bid>> GetBidsByAuctionId(Guid auctionId)
    {
        var filter = Builders<Bid>.Filter.Eq(b => b.AuctionId, auctionId); 
        return await _context.Bids.Find(filter).ToListAsync();  
    }
    /// <summary>
    /// Recupera todas las pujas (<see cref="Bid"/>) asociadas a una lista de subastas identificadas por sus respectivos <see cref="Guid"/>.
    /// Aplica un filtro <c>In</c> para obtener las pujas correspondientes a los identificadores provistos.
    /// </summary>
    /// <param name="auctionIds">
    /// Lista de identificadores únicos de subasta (<see cref="List{Guid}"/>), utilizada para filtrar las pujas en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Bid}}"/> que representa la operación asincrónica de lectura desde MongoDB:
    /// retorna una lista de objetos <see cref="Bid"/> relacionados con las subastas indicadas, o una lista vacía si no se encuentran resultados.
    /// </returns>
    public async Task<List<Bid>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds)
    {
        var filter = Builders<Bid>.Filter.In(b => b.AuctionId, auctionIds);
        return await _context.Bids.Find(filter).ToListAsync();
    }
}
