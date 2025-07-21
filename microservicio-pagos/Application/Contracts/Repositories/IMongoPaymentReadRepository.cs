using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IMongoPaymentReadRepository
{
    Task<HttpStatusCode> AddPayment(Payment payment);
    Task<Payment> GetPaymentById(Guid paymentId);
    Task<HttpStatusCode> UpdatePayment(Payment payment);
    Task<List<Payment>> GetPendingPaymentsByUserId(Guid userId);

}
