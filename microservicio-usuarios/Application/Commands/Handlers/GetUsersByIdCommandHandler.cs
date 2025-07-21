using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.DTOs;
using MediatR;

namespace Application.Commands.Handlers;

public class GetUsersByIdCommandHandler : IRequestHandler<GetUsersByIdCommand, List<AuctionResultsUserDto>?>
{
    IUserService _userService;
    public GetUsersByIdCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetUsersByIdCommand"/> para obtener información de resultados de subastas 
    /// asociada a una lista de identificadores de usuarios.
    /// </summary>
    /// <param name="request">Comando que contiene la colección de identificadores de usuarios a consultar.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Una lista de objetos <see cref="AuctionResultsUserDto"/> con los datos correspondientes a cada usuario.
    /// Devuelve <c>null</c> si no se encuentra información.
    /// </returns>
    public async Task<List<AuctionResultsUserDto>?> Handle(GetUsersByIdCommand request, CancellationToken cancellationToken)
    {
        return await _userService.GetAuctionResultsUserByIdAsync(request.usersIds);
    }
}
