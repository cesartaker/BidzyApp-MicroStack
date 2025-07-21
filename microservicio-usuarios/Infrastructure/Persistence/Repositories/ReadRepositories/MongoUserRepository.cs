using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.MongoDTOs;
using Application.Exceptions;
using Domain.ValueObjects;
using Infrastructure.Persistence.Context;
using MongoDB.Driver;
using Org.BouncyCastle.Crypto;

namespace Infrastructure.Persistence.Repositories.ReadRepositories;

public class MongoUserRepository: IMongoUserRepository
{
    private readonly MongoDbContext _mongoContext;

    public MongoUserRepository(MongoDbContext mongoDbContext)
    {
        _mongoContext = mongoDbContext;
    }
    /// <summary>
    /// Inserta un nuevo usuario en la colección <c>Users</c> de la base de datos MongoDB de lectura.
    /// Si la operación es exitosa, devuelve <c>true</c>; si ocurre una excepción, la registra en consola y devuelve <c>false</c>.
    /// </summary>
    /// <param name="user">Objeto <see cref="MongoUserDto"/> que contiene los datos del usuario a insertar.</param>
    /// <returns>
    /// <c>true</c> si el usuario fue insertado correctamente; <c>false</c> si ocurrió un error durante la operación.
    /// </returns>
    public async Task<bool> AddUser(MongoUserDto user)
    {
        try
        {
            await _mongoContext.Users.InsertOneAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al insertar usuario:{ex.Message}");
            return false;
        }
    }
    /// <summary>
    /// Actualiza la información de un usuario existente en la colección <c>Users</c> de MongoDB.
    /// Utiliza un filtro por <c>Id</c> para reemplazar el documento correspondiente.
    /// En caso de error en la operación, lanza una excepción personalizada <see cref="TransactionFailureException"/>.
    /// </summary>
    /// <param name="user">Objeto <see cref="MongoUserDto"/> que contiene los datos actualizados del usuario.</param>
    /// <returns>
    /// <c>true</c> si el usuario fue actualizado correctamente.
    /// </returns>
    /// <exception cref="TransactionFailureException">
    /// Se lanza si ocurre un error durante la operación de reemplazo en MongoDB.
    /// </exception>
    public async Task<bool> UpdateUser(MongoUserDto user)
    {
        try
        {
            var filter = Builders<MongoUserDto>.Filter.Eq(u => u.Id, user.Id);
            await _mongoContext.Users.ReplaceOneAsync(filter, user);
            return true;
        }
        catch(MongoException ex)
        {
            throw new TransactionFailureException($"Error al actualizar datos del usuario en MongoDB:{ex.Message}");
        }
        
    }
    /// <summary>
    /// Obtiene un rol de la base de datos MongoDB según el nombre especificado.
    /// Realiza la consulta en la colección <c>Rols</c> usando un filtro por nombre.
    /// Si se encuentra el rol, lo transforma en un objeto <see cref="MongoRoleDto"/> y lo retorna.
    /// </summary>
    /// <param name="roleName">Nombre del rol que se desea consultar.</param>
    /// <returns>
    /// Un objeto <see cref="MongoRoleDto"/> si el rol fue encontrado; <c>null</c> en caso contrario.
    /// </returns>
    /// <exception cref="Exception">
    /// Se lanza si ocurre un error inesperado durante la consulta en la base de datos.
    /// </exception>
    public async Task<MongoRoleDto?> GetRole(string roleName)
    {
        try
        {
            var filter = Builders<MongoRoleDto>.Filter.Eq(r => r.Name, roleName);
            var role = await _mongoContext.Rols.Find(filter).FirstOrDefaultAsync();
            if (role != null)
            {
                return new MongoRoleDto(role.Id, role.PostgresID, role.Name);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener el rol: {ex.Message}", ex);
        }

    }
    /// <summary>
    /// Busca un usuario en la colección <c>Users</c> de MongoDB utilizando su dirección de correo electrónico.
    /// Si se encuentra el usuario, transforma el documento en un objeto <see cref="MongoUserResponseDto"/> y lo retorna.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario que se desea consultar.</param>
    /// <returns>
    /// Objeto <see cref="MongoUserResponseDto"/> con los datos del usuario si existe; de lo contrario, <c>null</c>.
    /// </returns>
    public async Task<MongoUserResponseDto?> GetUserByEmail(string email)
    {
        var filter = Builders<MongoUserDto>.Filter.Eq(u => u.Email, new Email(email));
        var user = await _mongoContext.Users.Find(filter).FirstOrDefaultAsync();
        if (user != null)
        {
            var response = new MongoUserResponseDto(user.Id, user.FirstName, user.MiddleName, user.LastName, user.SecondLastName,
                user.Email, user.PhoneNumber, user.Address, user.RoleId, user.RoleName);
            return response;
        }

        return null;
    }
    /// <summary>
    /// Consulta la colección <c>Users</c> en MongoDB para obtener información relacionada con resultados de subasta
    /// según una lista de identificadores de usuario. Construye una lista de objetos <see cref="AuctionResultsUserDto"/>
    /// con los datos esenciales del usuario (ID, nombre y correo electrónico).
    /// </summary>
    /// <param name="id">Lista de identificadores únicos de los usuarios a consultar.</param>
    /// <returns>
    /// Una lista de objetos <see cref="AuctionResultsUserDto"/> con los datos requeridos para resultados de subasta.
    /// </returns>
    public async Task<List<AuctionResultsUserDto>> GetAuctionResultsUserById(List<Guid> id)
    {
        var filter = Builders<MongoUserDto>.Filter.In(u => u.Id, id);
        var users = await _mongoContext.Users.Find(filter).ToListAsync();

        var responseList = users.Select(user => new AuctionResultsUserDto
        {
            userId = user.Id,  
            name = $"{user.FirstName} {user.LastName}",
            email = user.Email?.Value ?? string.Empty,
        }).ToList();

        return responseList;

    }
    /// <summary>
    /// Recupera un usuario desde la colección <c>Users</c> en MongoDB utilizando su identificador único.
    /// Realiza la consulta mediante un filtro por <c>Id</c> y devuelve el primer resultado coincidente.
    /// </summary>
    /// <param name="id">Identificador único del usuario que se desea buscar.</param>
    /// <returns>
    /// Un objeto <see cref="MongoUserDto"/> con los datos del usuario si se encuentra; <c>null</c> en caso contrario.
    /// </returns>
    public async Task<MongoUserDto> GetUserById(Guid id)
    {
        var filter = Builders<MongoUserDto>.Filter.Eq(u => u.Id, id);
        return await _mongoContext.Users.Find(filter).FirstOrDefaultAsync();
    }
}
