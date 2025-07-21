using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.MongoDTOs;
using MediatR;

namespace Application.Commands;

public record GetUserInformationCommand(Guid userId):IRequest<MongoUserDto>;

