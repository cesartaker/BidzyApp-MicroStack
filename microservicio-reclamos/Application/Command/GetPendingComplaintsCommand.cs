using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;

namespace Application.Command;

public record GetPendingComplaintsCommand():IRequest<List<ComplaintCreatedDto>>;

