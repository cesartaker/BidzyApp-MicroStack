using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Services;

public interface IProductService
{
    Task<HttpStatusCode> AddProductAsync(Product product);
    Task<List<Product>> GetProductsAsync(Guid auctioneerId, ProductStatus status);
    Task<List<AuctionProductDto>> GetProductsByIdAsync(List<Guid> productIds);
    Task <Product> UpdateProductStatus(Guid productId,ProductStatus status);
}
