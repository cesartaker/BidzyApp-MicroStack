using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;

namespace Application.Commands;

public record GetPendingPaymentsCommand(Guid userId):IRequest<List<PaymentCreatedDto>>;

