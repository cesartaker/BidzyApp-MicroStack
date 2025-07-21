using Application.Builders;
using Application.Commands;
using Application.Contracts.Builders;
using Application.Contracts.Repositories;
using Application.Contracts.Sagas;
using Application.Contracts.Services;
using Application.Sagas;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.Bids;
using Infrastructure.Services.Notifications;
using Infrastructure.Services.Payments;
using Infrastructure.Services.Products;
using Infrastructure.Services.Users;
using Infrastructure.Sheduling;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<AuctionCreatedConsumer>();
    config.AddConsumer<AuctionUpdatedConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("auction_added_queue", e =>
        {
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("auction_updated_queue", e =>
        {
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });

    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateAuctionCommand).Assembly,
    typeof(CloseAuctionCommand).Assembly,
    typeof(GetActiveAuctionsCommand).Assembly,
    typeof(GetAuctionResultsCommand).Assembly,
    typeof(GetMyAuctionsCommand).Assembly,
    typeof(SendNotificationCommand).Assembly,
    typeof(SetAuctionCompletedCommand).Assembly,
    typeof(CreatePaymentCommand).Assembly));

// Add services to the container.

//Configurations
builder.Services.Configure<BidsServiceOptions>(builder.Configuration.GetSection("BidsService"));
builder.Services.Configure<UsersServiceOptions>(builder.Configuration.GetSection("UsersService"));
builder.Services.Configure<ProductsServiceOptions>(builder.Configuration.GetSection("ProductsService"));
builder.Services.Configure<NotificationServiceOptions>(builder.Configuration.GetSection("NotificationService"));
builder.Services.Configure<PaymentServiceOptions>(builder.Configuration.GetSection("PaymentsService"));


//Database Contexts
builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));

//Validators
builder.Services.AddScoped<IValidator<CreateAuctionCommand>, CreateAuctionCommandValidator>();
builder.Services.AddScoped<IValidator<ClaimPrizeCommand>, ClaimPrizeCommandValidator>();

//Repositories
builder.Services.AddScoped<IMongoAuctionWriteRepository, MongoAuctionWriteRepository>();
builder.Services.AddScoped<IMongoAuctionReadRepository, MongoAuctionReadRepository>();
builder.Services.AddScoped<IMongoWaybillWriteRepository, MongoWaybillWriteRepository>();
builder.Services.AddScoped<IMongoWaybillReadRepository, MongoWaybillReadRepository>();

//Application Services
builder.Services.AddScoped<IAuctionService,AuctionService>();

//Builders
builder.Services.AddScoped<IAuctionBuilder, AuctionBuilder>();
builder.Services.AddScoped<INotificationBuilder, NotificationBuilder>();

//Infrastructure Services   
builder.Services.AddScoped<IAuctionSheduler, HangfireAuctionSheduler>();
builder.Services.AddScoped<IBidsService, BidsServiceClient>();
builder.Services.AddScoped<IUserService, UsersServiceClient>();
builder.Services.AddScoped<IProductService, ProductsServiceClient>();
builder.Services.AddScoped<ICloseAuctionSagaStarter, CloseAuctionSagaStarter>();
builder.Services.AddScoped<INotificationService, NotificationServiceClient>();
builder.Services.AddScoped<IPaymentService, PaymentServiceClient>();
builder.Services.AddScoped<IPrizeService, PrizeService>();

// Add services to the container.

var cs = configuration.GetConnectionString("Hangfire");
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
        {
       options.UseNpgsqlConnection(cs); 
                                        
   });
});


builder.Services.AddHangfireServer();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
