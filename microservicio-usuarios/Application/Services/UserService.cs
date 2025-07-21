using Application.Contracts;
using Domain.Enums;
using Domain.Entities;
using Application.DTOs.MongoDTOs;
using Application.Exceptions;
using System.Net;
using Application.DTOs;
using Application.Contracts.Services;
using Application.Contracts.Repositories;

namespace Application.Services;

public class UserService: IUserService
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IPostgreUserRepository _postgreRepository;
    public readonly IMongoUserRepository _mongoRepository;
    public readonly IEmailService _emailService;

    public UserService(IUnitOfWork unitOfWork, IPostgreUserRepository postgreRepository, 
        IMongoUserRepository userRepository, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _postgreRepository = postgreRepository;
        _mongoRepository = userRepository;
        _emailService = emailService;
       
    }
    /// <summary>
    /// Registra un nuevo usuario en la base de datos PostgreSQL dentro de una transacción.
    /// Si la operación es exitosa, se confirma la transacción y se devuelve el código de estado HTTP correspondiente.
    /// En caso de error, se realiza rollback y se lanza una excepción personalizada.
    /// </summary>
    /// <param name="user">Objeto <see cref="User"/> que contiene la información del usuario a registrar.</param>
    /// <returns>
    /// Un valor <see cref="HttpStatusCode"/> que indica el resultado de la operación.
    /// </returns>
    /// <exception cref="TransactionFailureException">
    /// Se lanza si ocurre un error al registrar el usuario, lo que provoca el rollback de la transacción.
    /// </exception>
    public async Task<HttpStatusCode> AddUserAsync(User user) 
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
             _postgreRepository.AddUser(user);
            var result = await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw new TransactionFailureException($"Error: No se pudo registrar al usuario en la base de datos");

        }
    }
    /// <summary>
    /// Actualiza la información de un usuario existente en la base de datos PostgreSQL dentro de una operación transaccional.
    /// Si la actualización es exitosa, confirma la transacción y devuelve el código de estado HTTP.
    /// En caso de error, revierte los cambios y lanza una excepción personalizada.
    /// </summary>
    /// <param name="user">Objeto <see cref="User"/> que contiene los nuevos datos del usuario.</param>
    /// <returns>
    /// Un valor <see cref="HttpStatusCode"/> que indica el resultado de la operación.
    /// </returns>
    /// <exception cref="TransactionFailureException">
    /// Se lanza si ocurre un error al intentar actualizar al usuario, causando el rollback de la transacción.
    /// </exception>
    public async Task<HttpStatusCode> UpdateUserAsync(User user)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _postgreRepository.UpdateUser(user);
            var result = await _unitOfWork.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw new TransactionFailureException($"Error: No se pudo actualizar la información del usuairo");
        }
    }
    /// <summary>
    /// Obtiene el rol correspondiente a un nombre especificado desde la base de datos MongoDB.
    /// </summary>
    /// <param name="roleName">Nombre del rol que se desea buscar.</param>
    /// <returns>
    /// Un objeto <see cref="MongoRoleDto"/> si se encuentra el rol, o <c>null</c> si no existe.
    /// </returns>
    public async Task<MongoRoleDto?> GetRoleAsync(string roleName)
    {

        var role = await _mongoRepository.GetRole(roleName);
        if (role != null) 
        {
            return role;
        }
      return null;
    }
    /// <summary>
    /// Verifica si existe un usuario registrado en la base de datos MongoDB asociado al correo electrónico proporcionado.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario que se desea consultar.</param>
    /// <returns>
    /// <c>true</c> si el usuario está registrado; <c>false</c> en caso contrario.
    /// </returns>
    public async Task<bool> UserExistAsync(string email)
    {
        var userRegistered = await _mongoRepository.GetUserByEmail(email);
        if (userRegistered != null)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Obtiene los datos de un usuario a partir de su dirección de correo electrónico mediante consulta a MongoDB.
    /// Si el usuario no existe, lanza una excepción <see cref="NotFoundException"/>.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del usuario a buscar.</param>
    /// <returns>
    /// Objeto <see cref="MongoUserResponseDto"/> con la información del usuario encontrado.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Se lanza si no se encuentra ningún usuario asociado al correo electrónico proporcionado.
    /// </exception>
    /// <exception cref="Exception">
    /// Se lanza si ocurre un error inesperado durante el proceso de consulta.
    /// </exception>
    public async Task<MongoUserResponseDto> GetUser(string email)
    {
        try
        {
            var user = await _mongoRepository.GetUserByEmail(email);
            if (user != null)
            {
                return user;
            }
            throw new NotFoundException($"No se encontro ningun usuario con el correo: {email}");
        }
        catch (Exception) {
            throw;
        }
        
        
    }
    /// <summary>
    /// Envía un correo electrónico al destinatario especificado con el asunto y cuerpo proporcionados.
    /// Utiliza el servicio de correo electrónico configurado.
    /// </summary>
    /// <param name="recipient">Dirección de correo electrónico del destinatario.</param>
    /// <param name="subject">Asunto del correo.</param>
    /// <param name="body">Contenido del mensaje.</param>
    public async Task sendEmailToUser(string recipient, string subject, string body)
    {
        try
        {
            await _emailService.SendEmailAsync(recipient, subject, body);
            Console.WriteLine("Correo enviado con éxito");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar correo: {ex.Message}");
        }
    }
    /// <summary>
    /// Recupera una lista de usuarios suministrados.
    /// Realiza la consulta en la base de datos MongoDB mediante el repositorio correspondiente.
    /// </summary>
    /// <param name="ids">Lista de identificadores únicos de los usuarios cuyos datos se desean obtener.</param>
    /// <returns>
    /// Una lista de objetos <see cref="AuctionResultsUserDto"/> que contienen la información de los resultados de subasta por usuario.
    /// </returns>
    public async Task<List<AuctionResultsUserDto>> GetAuctionResultsUserByIdAsync(List<Guid> ids)
    {
        return await _mongoRepository.GetAuctionResultsUserById(ids);
    }
    /// <summary>
    /// Recupera la información de un usuario desde la base de datos MongoDB utilizando su identificador único.
    /// Delega la operación al repositorio correspondiente.
    /// </summary>
    /// <param name="id">Identificador único del usuario que se desea consultar.</param>
    /// <returns>
    /// Un objeto <see cref="MongoUserDto"/> que contiene los datos del usuario correspondiente.
    /// </returns>
    public Task<MongoUserDto> GetUserById(Guid id)
    {
        return _mongoRepository.GetUserById(id);
    }
}
