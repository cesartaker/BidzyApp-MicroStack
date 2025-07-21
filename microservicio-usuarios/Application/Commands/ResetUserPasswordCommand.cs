using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using FluentValidation;
using MediatR;

namespace Application.Commands;

public record ResetUserPasswordCommand(string Email):IRequest<HttpStatusCode>;
