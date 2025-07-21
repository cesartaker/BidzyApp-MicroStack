using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands;

public record SendEmailNotificationCommand(
    string email,
    string message,
    string subject
    ):IRequest<Unit>;

