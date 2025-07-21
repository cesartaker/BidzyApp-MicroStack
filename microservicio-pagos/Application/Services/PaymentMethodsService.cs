using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Domain.Entities;
using Stripe;

namespace Application.Services;

public class PaymentMethodsService : IPaymentMethodService
{
    public readonly IStripeService _stripeService;
    public readonly IMongoPaymentMethodReadRepository _mongoPaymentReadRepository;
    public readonly IMongoPaymentMethodWriteRepository _mongoPaymentWriteRepository;
    public PaymentMethodsService(IStripeService  stripeService, IMongoPaymentMethodReadRepository read, IMongoPaymentMethodWriteRepository write)
    {
        _stripeService = stripeService;
        _mongoPaymentReadRepository = read;
        _mongoPaymentWriteRepository = write;
    }
    /// <summary>
    /// Agrega un nuevo método de pago para el usuario especificado.
    /// Invoca internamente <see cref="GetPaymentMethod"/> para obtener un método simulado desde Stripe,
    /// construye un objeto <see cref="UserPaymentMethod"/> con los detalles obtenidos,
    /// y lo almacena en el repositorio Mongo mediante <c>AddPaymentMethod</c>.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) que recibirá el nuevo método de pago.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{UserPaymentMethod}"/> que representa la operación asincrónica:
    /// retorna el objeto <see cref="UserPaymentMethod"/> creado y persistido.
    /// </returns>
    public async Task<UserPaymentMethod> AddPaymentMethod(Guid userId)
    {
        var paymentMethod = GetPaymentMethod();
        var userPaymentMethod = new UserPaymentMethod(userId, paymentMethod.Id, paymentMethod.Card.Brand,
            paymentMethod.Card.Last4, paymentMethod.Card.ExpMonth.ToString(), paymentMethod.Card.ExpYear.ToString());
       
        await _mongoPaymentWriteRepository.AddPaymentMethod(userPaymentMethod);
        return userPaymentMethod;
    }
    /// <summary>
    /// Recupera todos los métodos de pago registrados para un usuario específico desde el repositorio de lectura en MongoDB.
    /// Utiliza el identificador único del usuario para filtrar los resultados correspondientes.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyos métodos de pago se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{UserPaymentMethod}}"/> que representa la operación asincrónica de consulta:
    /// retorna una lista de objetos <see cref="UserPaymentMethod"/> asociados al usuario, o una lista vacía si no se encuentran registros.
    /// </returns>
    public async Task<List<UserPaymentMethod>> GetPaymentMethodsByUserId(Guid userId)
    {
        return await _mongoPaymentReadRepository.GetPaymentMethodsByUserId(userId);
    }
    /// <summary>
    /// Genera aleatoriamente un identificador de método de pago (`pm_card_visa` o `pm_card_mastercard`)
    /// y utiliza el servicio de Stripe para recuperar los datos correspondientes.
    /// Este método está diseñado principalmente para pruebas, simulaciones o comportamientos de ejemplo.
    /// </summary>
    /// <returns>
    /// Un objeto <see cref="PaymentMethod"/> que contiene los detalles del método de pago seleccionado de forma aleatoria.
    /// </returns>
    public PaymentMethod GetPaymentMethod()
    {
        var random = new Random();
        var option = random.Next(0, 2);
        var pmi = option == 0 ? "pm_card_visa" : "pm_card_mastercard";
        return _stripeService.GetPaymentMethod(pmi);
    }
}
