using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;

namespace Application.Services;

public class ProductService : IProductService
{
    public readonly IMongoProductWriteRepository _writeRepository;
    public readonly IMongoProductReadRepository _readRepository;

    public ProductService(IMongoProductWriteRepository write, IMongoProductReadRepository read)
    {
        _writeRepository = write;
        _readRepository = read;
    }

    public async Task<HttpStatusCode> AddProductAsync(Product product)
    {
        var response = await _writeRepository.AddProduct(product);
        return response;
    }
    /// <summary>
    /// Guarda un nuevo producto en el repositorio de escritura.
    /// </summary>
    /// <param name="product">
    /// Objeto <see cref="Product"/> que contiene la información del producto a persistir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un <see cref="HttpStatusCode"/> indicando el resultado de la operación:
    /// por ejemplo, <c>HttpStatusCode.OK</c> si se guarda correctamente.
    /// </returns>
    public async Task<List<Product>> GetProductsAsync(Guid auctioneerId, ProductStatus status)
    {
        var products = await _readRepository.GetProductsByUserAndStatus(auctioneerId,status);
        return products;
    }
    /// <summary>
    /// Recupera una lista de productos en formato <see cref="AuctionProductDto"/> a partir de sus identificadores únicos.
    /// Utiliza el repositorio de lectura para realizar la consulta.
    /// </summary>
    /// <param name="productIds">
    /// Lista de identificadores <see cref="Guid"/> que representan los productos a recuperar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en una lista de <see cref="AuctionProductDto"/> con la información de los productos solicitados.
    /// </returns>
    public async Task<List<AuctionProductDto>> GetProductsByIdAsync(List<Guid> productIds)
    {
        return await _readRepository.GetProductsById(productIds);
    }
    /// <summary>
    /// Actualiza el estado de un producto en el repositorio de escritura utilizando su identificador.
    /// </summary>
    /// <param name="productId">
    /// Identificador único del producto que se desea actualizar.
    /// </param>
    /// <param name="status">
    /// Nuevo estado <see cref="ProductStatus"/> que se asignará al producto.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en el objeto <see cref="Product"/> actualizado con el nuevo estado.
    /// </returns>
    public async Task<Product> UpdateProductStatus(Guid productId, ProductStatus status)
    {
        return await _writeRepository.UpdateProductStatus(productId, status);
    }
}
