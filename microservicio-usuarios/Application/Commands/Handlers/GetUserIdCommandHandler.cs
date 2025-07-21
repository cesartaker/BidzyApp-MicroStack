using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using MediatR;

namespace Application.Commands.Handlers;

public class GetUserIdCommandHandler: IRequestHandler<GetUserIdCommand,Guid>
{
    private readonly IUserService _userService;

    public GetUserIdCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetUserIdCommand"/> para obtener el identificador único de un usuario
    /// a partir de su dirección de correo electrónico.
    /// </summary>
    /// <param name="request">Comando que contiene el correo electrónico del usuario.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Un valor <see cref="Guid"/> que representa el identificador del usuario asociado al correo suministrado.
    /// </returns>
    public async Task<Guid> Handle(GetUserIdCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.GetUser(request.email);
        return userId.Id;
    }
}
