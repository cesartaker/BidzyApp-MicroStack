using Application.Commands;
using Application.Contracts;
using Application.Services;
using bids_api.Helpers;
using Domain.Contracts.Repositories;
using Domain.Contracts.Services;
using Infrastructure.Cache;
using Infrastructure.ExternalServices.AuctionService;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.WebSockets;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();



// Add services to the container.

//Configurations

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<BidAddedConsumer>();
    config.AddConsumer<AuctionClosedConsumer>();
    config.AddConsumer<AuctionAddedConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("bid-added-queue", e =>
        {
            e.ConfigureConsumer<BidAddedConsumer>(context);
        });

        cfg.ReceiveEndpoint("auction_closed_queue", e =>
        {
            e.ConfigureConsumer<AuctionClosedConsumer>(context);
        });

        cfg.ReceiveEndpoint("auction_added_bids_queue", e =>
        {
            e.ConfigureConsumer<AuctionAddedConsumer>(context);
        });

    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(NewBidCommand).Assembly,
    typeof(GetBidsCommand).Assembly));


builder.Services.Configure<AuctionServiceOptions>(builder.Configuration.GetSection("AuctionsService"));

//WebSockets
builder.Services.AddSingleton<IBidsSocketHub, BidsSocketHub>();

//Database Contexts
builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));

//Cache
builder.Services.AddSingleton<ICacheAuctionsStates, InMemoryAuctionCache>();
builder.Services.AddHostedService<CacheInitializationService>();

//Repositories
builder.Services.AddScoped<IMongoBidsReadRepository, MongoBidsReadRepository>();
builder.Services.AddScoped<IMongoBidsWriteRepository, MongoBidsWriteRepository>();

//Application Services
builder.Services.AddScoped<IBidsService, BidsService>();
builder.Services.AddSingleton<IAutoBidsService, AutoBidsService>();

//Infrastructure Services
builder.Services.AddSingleton<IAuctionsService, AuctionServiceClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.Map("/ws/bids", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var accessToken = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Token de autenticación requerido");
            return;
        }
        var userId = JwtTokenHelper.ExtractUserIdFromToken(accessToken);
        var userName = JwtTokenHelper.ExtractUserNameFromToken(accessToken);
        var auctionId = context.Request.Query["auctionId"].ToString();
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        var handler = context.RequestServices.GetRequiredService<IBidsSocketHub>();
        await handler.HandleWebSocketAsync(socket, auctionId, userId,userName);
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("WebSocket request expected.");
    }
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
