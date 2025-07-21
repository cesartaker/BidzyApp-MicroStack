using Application.Services;
using Application.Contracts.Services;
using Application.Commands;
using FluentValidation;
using Application.Validators;
using Infrastructure.External.Cloud;
using Infrastructure.Persistence.Contexts;
using MassTransit;
using Infrastructure.Messaging.Consumers;
using Application.Contracts.Repositories;
using Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<AddProductConsumer>();
    config.AddConsumer<UpdateProductStatusConsumer>();


    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("products_queue", e =>
        {
            e.ConfigureConsumer<AddProductConsumer>(context);
        });

        cfg.ReceiveEndpoint("updated_product_status_queue", e =>
        {
            e.ConfigureConsumer<UpdateProductStatusConsumer>(context);
        });


    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(RegisterProductCommand).Assembly,
    typeof(GetProductsCommand).Assembly));

// Add services to the container.

builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));


builder.Services.AddScoped<IValidator<RegisterProductCommand>, RegisterProductCommandValidator>();
builder.Services.AddScoped<IValidator<GetProductsCommand>, GetProductsCommandValidator>();

builder.Services.AddScoped<IMongoProductReadRepository, MongoProductReadRepository>();
builder.Services.AddScoped<IMongoProductWriteRepository, MongoProductWriteRepository>();



builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IProductService, ProductService>();


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
