using System.Text.Json.Serialization;
using Application.Command;
using Application.Contracts.Respositories;
using Application.Contracts.Services;
using Application.Services;
using Infrastructure.ExternalServices.Cloud;
using Infrastructure.ExternalServices.Notifications;
using Infrastructure.ExternalServices.Users;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
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
    config.AddConsumer<ComplaintCreatedConsumer>();
    config.AddConsumer<ComplaintUpdatedConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("complaints_queue", e =>
        {
            e.ConfigureConsumer<ComplaintCreatedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("complaint_updated_queue", e =>
        {
            e.ConfigureConsumer<ComplaintUpdatedConsumer>(context);
        });

    });
});
//MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateComplaintCommand).Assembly));

//Contexts
builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));

//Services
builder.Services.AddScoped<IComplaintService,ComplaintService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

//Repositories
builder.Services.AddScoped<IMongoComplaintWriteRepository, MongoComplaintWriteRepository>();
builder.Services.AddScoped<IMongoComplaintReadRespository, MongoComplaintReadRepository>();

//Configuration
builder.Services.Configure<NotificationServiceOptions>(builder.Configuration.GetSection("NotificationService"));
builder.Services.Configure<UsersServiceOptions>(builder.Configuration.GetSection("UsersService"));

//Services
builder.Services.AddScoped<INotificationService, NotificationServiceClient>();
builder.Services.AddScoped<IUserService, UsersServiceClient>();



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
