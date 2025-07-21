using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.DTOs.MongoDTOs;
using MediatR;

namespace Application.Commands.Handlers;

public class GetUserInformationCommandHandler : IRequestHandler<GetUserInformationCommand, MongoUserDto>
{
    private readonly IUserService _userService;

    public GetUserInformationCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Maneja el comando <see cref="GetUserInformationCommand"/> para obtener los datos de un usuario a partir de su identificador.
    /// Invoca el servicio correspondiente para consultar la información en la fuente de datos.
    /// </summary>
    /// <param name="request">Comando que contiene el <c>userId</c> del usuario a consultar.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    /// <returns>
    /// Un objeto <see cref="MongoUserDto"/> con la información del usuario solicitado.
    /// </returns>
    public async Task<MongoUserDto> Handle(GetUserInformationCommand request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserById(request.userId);
    }
}
