using Application.Commands;
using Application.Services;
using Application.Validators;
using Domain.Contracts.Repositories;
using Domain.Contracts.Services;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services.EmailService;
using MassTransit;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<SubmittedNotificationEventConsumer>();
   

    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(configuration["RabbitMq:Connection"], h =>
        {
            h.Username(configuration["RabbitMq:User"]!);
            h.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("new_notification_queue", e =>
        {
            e.ConfigureConsumer<SubmittedNotificationEventConsumer>(context);
        });

        
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(SendEmailNotificationCommand).Assembly));

builder.Services.Configure<EmailServiceOptions>(configuration.GetSection("EmailService"));

// Add services to the container.

builder.Services.AddControllers();

//Validators
builder.Services.AddScoped<IValidator<SendEmailNotificationCommand>, SendEmailNotificationCommandValidator>();

//Database Contexts
builder.Services.AddSingleton(new MongoDbWriteContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseWriteName"]!));
builder.Services.AddSingleton(new MongoDbReadContext(configuration["ConnectionStrings:MongoDB"]!, configuration["MongoSettings:DatabaseReadName"]!));

//Repositories
builder.Services.AddSingleton<IMongoNotificationReadRepository,MongoNotificationReadRepository>();
builder.Services.AddSingleton<IMongoNotificationWriteRepository, MongoNotificationWriteRepository>();

//Application Services
builder.Services.AddScoped<INotificationService, NotificationService>();
//Infrastructure Services
builder.Services.AddScoped<IEmailNotification,EmailService>();

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
