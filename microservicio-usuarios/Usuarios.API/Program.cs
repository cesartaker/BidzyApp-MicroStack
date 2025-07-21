using Microsoft.EntityFrameworkCore;
using Application.Commands;
using Application.Contracts;
using Infrastructure.Persistence;
using FluentValidation;
using Application.Validators;
using MassTransit;
using Infrastructure.Messaging.Cosumers;
using Infrastructure.Persistence.Context;
using Application.Services;
using DotNetEnv;
using Infrastructure.Services;
using Infrastructure.Persistence.Repositories.WriteRepositories;
using Infrastructure.Persistence.Repositories.ReadRepositories;
using Application.Contracts.Services;
using Application.Contracts.Repositories;


//Loading Enviroment Variables

Env.Load("../Infrastructure/.env");


var builder = WebApplication.CreateBuilder(args);

//Add ConfigurationBuilder Settings

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"),
            b => b.MigrationsAssembly("Usuarios.API")));


// Masstransit Configuration
builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    config.AddConsumer<MongoCreateUserConsumer>();
    config.AddConsumer<MongoUpdateUserConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(Environment.GetEnvironmentVariable("RABBIT_HOST")!, h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USER")!);
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASSWORD")!);
        });

        cfg.ReceiveEndpoint("users_queue", e =>
        {
            e.ConfigureConsumer<MongoCreateUserConsumer>(context);
        });
        cfg.ReceiveEndpoint("update_users_queue", e =>
        {
            e.ConfigureConsumer<MongoUpdateUserConsumer>(context);
        });
       
    });
});


//Validator 
builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UpdateUserCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(ResetUserPasswordCommandValidator).Assembly);


// Singleton
builder.Services.AddSingleton(new MongoDbContext(Environment.GetEnvironmentVariable("MONGO_CONNECTION")!,
    Environment.GetEnvironmentVariable("MONGO_DB_NAME")!));
builder.Services.AddSingleton<IEmailService,EmailService>();

// Scoped
builder.Services.AddScoped<IPostgreUserRepository, PostgreUserRepository>();
builder.Services.AddScoped<IPostgreUserActivityHistoryRepository, PostgreUserActivityHistoryRepository>();

builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IMongoRoleRepository, MongoRoleRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IKeycloackService,KeycloakServices>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAuditService, UserAuditService>();

builder.Services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
builder.Services.AddScoped<IValidator<ResetUserPasswordCommand>, ResetUserPasswordCommandValidator>();

// MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateUserCommand).Assembly,
    typeof(UpdateUserCommand).Assembly,
    typeof(ResetUserPasswordCommand).Assembly,
    typeof(UpdateUserPasswordCommand).Assembly,
    typeof(GetUserActivityHistoryCommand).Assembly,
    typeof(GetUserIdCommand).Assembly,
    typeof(GetUsersByIdCommand).Assembly));

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
