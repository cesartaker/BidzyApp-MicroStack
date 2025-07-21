using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using MediatR;
using Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using Application.Dtos;
using Domain.Contracts.Services;
using Application.Contracts;
using Application.Exceptions;

namespace Infrastructure.WebSockets;

public class BidsSocketHub : IBidsSocketHub
{

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<WebSocket, Guid>> _channels = new();
    private readonly IServiceScopeFactory _scopeFactory;

    public BidsSocketHub(IServiceScopeFactory serviceScopeFactory)
    {
        _scopeFactory = serviceScopeFactory;
    }
    /// <summary>
    /// Administra la comunicación en tiempo real con un cliente WebSocket dentro del contexto de una subasta.
    /// Verifica si la subasta está activa y agrega el socket al canal correspondiente.
    /// Envía el historial de pujas al cliente recién conectado.
    /// Escucha mensajes entrantes (pujas nuevas y configuraciones de pujas automáticas), y reacciona según el tipo.
    /// Publica las pujas válidas al resto de los clientes y desencadena el procesamiento de pujas automáticas.
    /// Cierra el socket si la subasta no está activa o si se interrumpe la conexión.
    /// </summary>
    /// <param name="socket">
    /// Conexión <see cref="WebSocket"/> activa con el cliente participante.
    /// </param>
    /// <param name="auctionId">
    /// Identificador único (<see cref="string"/>) de la subasta en la que participa el cliente.
    /// </param>
    /// <param name="bidderId">
    /// Identificador único (<see cref="string"/>) del postor que se conecta.
    /// </param>
    /// <param name="userName">
    /// Nombre del usuario (postor) que será referenciado en las pujas y notificaciones.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica del manejo completo del ciclo de vida WebSocket.
    /// </returns>
    public async Task HandleWebSocketAsync(WebSocket socket, string auctionId, string bidderId, string userName)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheAuctionsStates>();
        var autobidService = scope.ServiceProvider.GetRequiredService<IAutoBidsService>();

        if (!cache.IsActive(auctionId))
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Subasta no activa", CancellationToken.None);
            return;
        }

        _channels.TryAdd(auctionId, new ConcurrentDictionary<WebSocket, Guid>());
        _channels[auctionId].TryAdd(socket, Guid.Parse(bidderId));

        try
        {
            //Send the current bids history to the newly connected client
            var history = await mediator.Send(new GetBidsCommand(Guid.Parse(auctionId)));
            var jsonHistory = JsonSerializer.Serialize(history);
            await socket.SendAsync(Encoding.UTF8.GetBytes(jsonHistory), WebSocketMessageType.Text, true, CancellationToken.None);

            // Listen for incoming messages from the WebSocket
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine(" Mensaje vacío recibido. Ignorando...");
                    continue; 
                }
                var envelope = JsonSerializer.Deserialize<WebSocketEnvelope>(json);

                switch (envelope?.type)
                {
                    case "new_bid":


                        var command = envelope.payload.Deserialize<NewBidCommand>();
                        command!.AuctionId = Guid.Parse(auctionId);
                        command!.BidderId = Guid.Parse(bidderId);
                        command!.bidderName = userName;
                        var response = await mediator.Send(command);
                        if (response != null)
                        {
                            var message = JsonSerializer.Serialize(response);
                            await BroadcastAsync(auctionId, message);
                            await autobidService.ExecuteAutoBids(Guid.Parse(auctionId));
                        }
                        else
                        {
                            var errorMessage = JsonSerializer.Serialize(new
                            {
                                type = "bid_error",
                                message = "Tu puja no fue valida. Intenta nuevamente."
                            });

                            await socket.SendAsync(
                              Encoding.UTF8.GetBytes(errorMessage),
                              WebSocketMessageType.Text,
                              true,
                              CancellationToken.None
                            );
                        }
                        break;

                    case "auto_bids":
                        var autoBidConfig = envelope.payload.Deserialize<AutoBidConfigDto>();
                        var autoBidService = scope.ServiceProvider.GetRequiredService<IAutoBidsService>();
                        string? notification = string.Empty;
                        try
                        {
                            await autoBidService.RegisterAutoBid(
                            Guid.Parse(auctionId),
                            Guid.Parse(bidderId),
                            userName,
                            autoBidConfig.MaxAmount,
                            autoBidConfig.MinBidIncrease
                            );

                            notification = JsonSerializer.Serialize(new
                            {
                                type = "auto_bid_registered",
                                payload = new
                                {
                                    bidderName = userName,
                                    maxAmount = autoBidConfig.MaxAmount,
                                    minBidIncrease = autoBidConfig.MinBidIncrease
                                }
                            });


                        }
                        catch (MinBidIncreaseException)
                        {
                            notification = JsonSerializer.Serialize(new
                            {
                                type = "auto_bid_error",
                                message = "El monto mínimo de incremento indicado es menor al incremento mínimo de la subasta por puja."
                            });
                        }
                        finally
                        {
                            await socket.SendAsync(
                                Encoding.UTF8.GetBytes(notification),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            );
                        }


                        break;



                    default:
                        Console.WriteLine($"Unknown message type: {envelope?.type}");
                        break;
                }
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            Console.WriteLine($"Cliente desconectado sin handshake: {auctionId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling WebSocket for auction {auctionId}: {ex.Message}");
        }
        finally
        {
            _channels[auctionId].TryRemove(socket, out _);
        }

    }
    /// <summary>
    /// Envía un mensaje a todos los clientes conectados mediante WebSocket que están participando en una subasta específica.
    /// Itera sobre los sockets activos asociados al <paramref name="auctionId"/> y transmite el mensaje indicado.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="string"/>) cuyos clientes deben recibir el mensaje.
    /// </param>
    /// <param name="message">
    /// Contenido del mensaje (<see cref="string"/>) que será enviado a los clientes mediante WebSocket.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de difusión del mensaje.
    /// </returns>
    private async Task BroadcastAsync(string auctionId, string message)
    {
        foreach (var socket in _channels[auctionId].Keys.Where(ws => ws.State == WebSocketState.Open))
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    /// <summary>
    /// Envía la información de una puja automática realizada por un postor a todos los clientes WebSocket conectados a una subasta específica.
    /// Serializa el contenido como un mensaje tipo <c>"new_bid"</c> con el nombre del postor y el monto ofrecido,
    /// y lo transmite a cada socket que permanece abierto dentro del canal correspondiente.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="string"/>) en la que se realizó la puja automática.
    /// </param>
    /// <param name="bid">
    /// Objeto <see cref="BidResponseDto"/> que contiene los datos de la puja (nombre del postor y monto ofertado).
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de difusión del mensaje entre todos los sockets abiertos.
    /// </returns>
    public async Task BroadcastAutoBidAsync(string auctionId, BidResponseDto bid)
    {
        if (!_channels.ContainsKey(auctionId))
            return;

        var message = JsonSerializer.Serialize(new
        {
            type = "new_bid",
            payload = new
            {
                bid.bidderName,
                bid.amount
            }
        });

        foreach (var socket in _channels.GetValueOrDefault(auctionId)?.Keys ?? Enumerable.Empty<WebSocket>())
        {

            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(
                    Encoding.UTF8.GetBytes(message),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }
    }
    /// <summary>
    /// Cierra todas las conexiones WebSocket activas o parcialmente cerradas asociadas a una subasta específica.
    /// Envía una notificación de cierre con motivo <c>"La subasta ha finalizado"</c> a cada socket correspondiente.
    /// Posteriormente, elimina el canal de comunicación asociado al <paramref name="auctionId"/> del diccionario de canales.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="string"/>) cuyas conexiones WebSocket deben ser cerradas.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de cierre de conexiones y limpieza del canal.
    /// </returns>
    public async Task CloseSocketsForAuctionAsync(string auctionId)
    {
        if (!_channels.TryGetValue(auctionId, out var sockets))
            return;

        foreach (var kvp in sockets)
        {
            var socket = kvp.Key;

            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "La subasta ha finalizado",
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cerrar socket: {ex.Message}");
                }
            }
        }
        _channels.TryRemove(auctionId, out _);
    }
}
