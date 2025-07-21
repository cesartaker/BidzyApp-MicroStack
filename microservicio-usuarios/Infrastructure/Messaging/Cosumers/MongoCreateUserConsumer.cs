
using Domain.Entities;
using MassTransit;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using Application.DTOs;
using Application.DTOs.MongoDTOs;
using Infrastructure.Persistence.Repositories;
using Application.Contracts.Repositories;
using Application.Contracts.Services;


namespace Infrastructure.Messaging.Cosumers;

public class MongoCreateUserConsumer: IConsumer<MongoUserDto>
{
    private readonly IMongoUserRepository _mongoUserRepository;
    private readonly IKeycloackService _keycloakService;

    public MongoCreateUserConsumer(IMongoUserRepository mongoUserRepository,IKeycloackService keycloackService)
    {
        _mongoUserRepository = mongoUserRepository;
        _keycloakService = keycloackService;
    }
    /// <summary>
    /// Procesa un mensaje del tipo <see cref="MongoUserDto"/> recibido desde la cola de mensajes.
    /// Intenta agregar el usuario recibido a MongoDB mediante el repositorio correspondiente.
    /// En caso de falla, registra el error en la consola.
    /// </summary>
    /// <param name="context">Contexto del mensaje que contiene la instancia de <see cref="MongoUserDto"/>.</param>
    public async Task Consume (ConsumeContext<MongoUserDto> context)
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
            
            bool result = await _mongoUserRepository.AddUser(user);

            if (!result)
                Console.WriteLine("Error: No se pudo agregar el usuario a MongoDB");
        }
        catch (Exception ex)
        {
            await _keycloakService.DeleteUserAsync(context.Message.Id);
            Console.WriteLine($"Excepción capturada en MongoUserConsumer Consume {ex.Message}");
        }
    }
}
