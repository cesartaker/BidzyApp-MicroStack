using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record SendPaymentCommand(Guid paymentId,string PaymentMethodId) : IRequest<Payment>;
