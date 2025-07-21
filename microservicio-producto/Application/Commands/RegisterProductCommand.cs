using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands;

public record RegisterProductCommand(Guid AuctioneerId, string Name, string Description, 
    decimal BasePrice,string Category, IFormFile Image): IRequest<RegisteredProductResponseDto>;