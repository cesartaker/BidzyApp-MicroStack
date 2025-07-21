using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Repositories;

public interface IMongoProductWriteRepository
{
    Task<HttpStatusCode> AddProduct(Product product);
    Task<Product> UpdateProductStatus(Guid productId, ProductStatus status);
}
