using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Command;

public record CreateComplaintCommand(Guid userId, string reason, string description, IFormFile? evidence):IRequest<ComplaintCreatedDto>;
