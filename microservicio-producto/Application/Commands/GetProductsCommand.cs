using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Commands;

public record GetProductsCommand(Guid auctioneerId, string status): IRequest<GetProductsResponseDto>;
