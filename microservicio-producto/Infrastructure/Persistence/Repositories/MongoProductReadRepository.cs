using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoProductReadRepository : IMongoProductReadRepository
{
    private readonly MongoDbReadContext _context;

    public MongoProductReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    public async Task<HttpStatusCode> AddProduct(Product product)
    {
        await _context.Products.InsertOneAsync(product);
        return HttpStatusCode.OK;
    }
    /// <summary>
    /// Recupera una lista de productos desde la base de datos utilizando una colección de identificadores.
    /// Aplica un filtro para buscar los productos y los transforma en objetos <see cref="AuctionProductDto"/> con información relevante.
    /// </summary>
    /// <param name="productIds">
    /// Lista de identificadores únicos <see cref="Guid"/> que representan los productos a consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en una lista de <see cref="AuctionProductDto"/> con los datos básicos de cada producto.
    /// </returns>
    public async Task<List<AuctionProductDto>> GetProductsById(List<Guid> productIds)
    {
        var filter = Builders<Product>.Filter.In(p => p.Id, productIds);
        var products = await _context.Products.Find(filter).ToListAsync();

        var result = products.Select(p => new AuctionProductDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            Description = p.Description
        }).ToList();

        return result;
    }
    /// <summary>
    /// Recupera una lista de productos desde la base de datos que pertenecen a un subastador específico
    /// y que coinciden con un estado determinado.
    /// Utiliza un filtro combinado para aplicar ambas condiciones.
    /// </summary>
    /// <param name="auctioneerId">
    /// Identificador único del subastador (<see cref="Guid"/>) cuyos productos se desean consultar.
    /// </param>
    /// <param name="status">
    /// Estado (<see cref="ProductStatus"/>) por el que se desea filtrar los productos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en una lista de objetos <see cref="Product"/> que cumplen con ambos criterios de búsqueda.
    /// </returns>
    public async Task<List<Product>> GetProductsByUserAndStatus(Guid auctioneerId, ProductStatus status)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.AuctioneerId, auctioneerId),
            Builders<Product>.Filter.Eq(p => p.Status, status)
        );

        return await _context.Products.Find(filter).ToListAsync();
    }
    /// <summary>
    /// Actualiza la información de un producto existente en la base de datos.
    /// Verifica que el producto sea válido, aplica el filtro por ID y realiza la operación de reemplazo.
    /// Si no se encuentra el producto, lanza una excepción personalizada.
    /// </summary>
    /// <param name="product">
    /// Objeto <see cref="Product"/> que contiene los datos actualizados del producto.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de actualización.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el objeto del producto es nulo o su ID no es válido.
    /// </exception>
    /// <exception cref="UpdateProductException">
    /// Se lanza si no se encuentra ningún producto con el ID especificado en la base de datos.
    /// </exception>
    public async Task UpdateProduct(Product product)
    {
        if (product == null || product.Id == Guid.Empty)
            throw new ArgumentException("Producto inválido");

        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        var resultado = await _context.Products.ReplaceOneAsync(filter, product);

        if (resultado.MatchedCount == 0)
            throw new UpdateProductException($"No se encontró el producto con Id: {product.Id}");
    }
}
