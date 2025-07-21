using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Persistence.Contexts;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoProductWriteRepository : IMongoProductWriteRepository
{
    private readonly MongoDbWriteContext _context;

    public MongoProductWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta un nuevo producto en la colección de productos en la base de datos.
    /// Verifica que el objeto no sea nulo antes de realizar la operación.
    /// Maneja posibles errores mediante captura de excepciones y retorna el estado HTTP correspondiente.
    /// </summary>
    /// <param name="product">
    /// Objeto <see cref="Product"/> que contiene la información del producto a insertar en la base de datos.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un <see cref="HttpStatusCode"/> indicando el resultado de la operación:
    /// <c>OK</c> si se inserta correctamente, <c>BadRequest</c> si el producto es nulo, o <c>InternalServerError</c> si ocurre una excepción.
    /// </returns>
    public async Task<HttpStatusCode> AddProduct(Product product)
    {
        try
        {
            if (product == null)
                return HttpStatusCode.BadRequest;

            await _context.Products.InsertOneAsync(product);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al insertar producto: {ex.Message}");
            return HttpStatusCode.InternalServerError;
        }
    }
    /// <summary>
    /// Actualiza el estado de un producto en la base de datos utilizando su identificador.
    /// Aplica un filtro para localizar el producto y actualiza su estado,
    /// devolviendo el documento modificado. Si ocurre una excepción, lanza una <see cref="UpdateProductException"/>.
    /// </summary>
    /// <param name="productId">
    /// Identificador único del producto que se desea actualizar.
    /// </param>
    /// <param name="status">
    /// Nuevo estado <see cref="ProductStatus"/> que se asignará al producto.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un objeto <see cref="Product"/> actualizado con el nuevo estado.
    /// </returns>
    /// <exception cref="UpdateProductException">
    /// Se lanza si ocurre un error durante la operación de actualización.
    /// </exception>
    public async Task<Product> UpdateProductStatus(Guid productId, ProductStatus status)
    {
        try
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
            var update = Builders<Product>.Update.Set(p => p.Status, status);

            var options = new FindOneAndUpdateOptions<Product>
            {
                ReturnDocument = ReturnDocument.After 
            };

            var updatedProduct = await _context.Products.FindOneAndUpdateAsync(filter, update, options);

            return updatedProduct;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar el estado del producto: {ex.Message}");
            throw new UpdateProductException();
        }
    }
}

