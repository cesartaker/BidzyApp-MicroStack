using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands;

public record UpdateUserPasswordCommand(
    string userId, string oldPassword,string newPassword, string token, string username): IRequest<HttpStatusCode>;
