using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Repositories;

public interface IMongoProductReadRepository
{
    Task<HttpStatusCode> AddProduct(Product product);

    Task<List<Product>> GetProductsByUserAndStatus(Guid auctioneerId, ProductStatus status);
    Task<List<AuctionProductDto>> GetProductsById(List<Guid> productIds);
    Task UpdateProduct(Product product);
}
