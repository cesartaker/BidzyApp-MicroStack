using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs.MongoDTOs;
using MassTransit;

namespace Infrastructure.Messaging.Cosumers;

public class MongoUpdateUserConsumer: IConsumer<MongoUserDto>
{
    private readonly IMongoUserRepository _mongoUserRepository;

    public MongoUpdateUserConsumer(IMongoUserRepository mongoUserRepository)
    {
        _mongoUserRepository = mongoUserRepository;
    }
    /// <summary>
    /// Procesa un mensaje del tipo <see cref="MongoUserDto"/> recibido desde la cola de mensajes.
    /// Intenta actualizar la información del usuario en MongoDB mediante el repositorio correspondiente.
    /// Si la operación falla, registra el error en la consola.
    /// </summary>
    /// <param name="context">Contexto del mensaje que contiene la instancia de <see cref="MongoUserDto"/>.</param>
    public async Task Consume(ConsumeContext<MongoUserDto> context)
    {
        try
        {
            
            MongoUserDto user = new MongoUserDto(
                context.Message.Id,
                context.Message.FirstName,
                context.Message.MiddleName,
                context.Message.LastName,
                context.Message.SecondLastName,
                context.Message.Email,
                context.Message.PhoneNumber,
                context.Message.Address,
                context.Message.RoleId,
                context.Message.RoleName
            );

            bool result = await _mongoUserRepository.UpdateUser(user);

            if (!result)
                Console.WriteLine("Error: No se pudo agregar el usuario a MongoDB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción capturada en MongoUpdateUserConsumer Consume: {ex.Message}");
        }
    }
}
