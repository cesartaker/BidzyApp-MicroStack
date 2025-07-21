using System.Text.Json.Serialization;
using Application.Commands;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Services;
using Application.Services.Options;
using Infrastructure.Messaging;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.Auctions;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<PaymentCreatedConsumer>();
    config.AddConsumer<PaymentUpdatedConsumer>();
    config.AddConsumer<PaymentMethodAddedConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("new_payment_queue", e =>
        {
            e.ConfigureConsumer<PaymentCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("update_payments_queue", e =>
        {
            e.ConfigureConsumer<PaymentUpdatedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("payment_method_added_queue", e =>
        {
             e.ConfigureConsumer<PaymentMethodAddedConsumer>(context);
        });
    });
});

//Options
builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<AuctionServiceOptions>(builder.Configuration.GetSection("AuctionService"));


//MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(SendPaymentCommand).Assembly,
    typeof(CreatePaymentCommand).Assembly,
    typeof(GetPendingPaymentsCommand).Assembly,
    typeof(AddPaymentMethodCommand).Assembly,
    typeof(GetPaymentMethodCommand).Assembly));

//Services
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodsService>();
builder.Services.AddScoped<IAuctionService, AuctionServiceClient>();

//Contexts
builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));

//Repositories
builder.Services.AddScoped<IMongoPaymentReadRepository, MongoPaymentReadRepository>();
builder.Services.AddScoped<IMongoPaymentWriteRepository, MongoPaymentWriteRepository>();
builder.Services.AddScoped<IMongoPaymentMethodReadRepository, MongoPaymentMethodReadRepository>();
builder.Services.AddScoped<IMongoPaymentMethodWriteRepository, MongoPaymentMethodWriteRepository>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
