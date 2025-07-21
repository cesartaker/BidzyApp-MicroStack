using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;

namespace Application.Contracts.Services;

public interface IProductService
{
    Task<List<AuctionProductDto>> GetProductsInformationByIds(List<Guid>? productIds);
    Task<HttpStatusCode> UpdateProductStatus(Guid productId, string status);
}
