using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IMongoPaymentWriteRepository
{
    Task<HttpStatusCode> AddPayment(Payment payment);
    Task<HttpStatusCode> UpdatePayment(Payment payment);

}
