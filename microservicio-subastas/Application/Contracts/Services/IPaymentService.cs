using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services;

public interface IPaymentService
{
    Task<HttpStatusCode> CreatePayment(Guid auctionId, Guid userId, decimal amount);
}
