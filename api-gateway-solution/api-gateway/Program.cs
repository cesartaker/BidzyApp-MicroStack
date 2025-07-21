
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

Env.Load(".env");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = Environment.GetEnvironmentVariable("KEYCLOAK_SERVER");
        options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
          ValidAudiences = builder.Configuration.GetSection("Authentication:Audiences").Get<List<string>>()
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // ← React frontend
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
//Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedOnly", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("ManageAuctionsPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            return permissions.Contains("manage-auctions");
        }));
    options.AddPolicy("ManageComplaintsPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            return permissions.Contains("manage-complaints");
        }));
    options.AddPolicy("PlaceComplaintsPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            return permissions.Contains("place-complaint");
        }));
    options.AddPolicy("BidderPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            return permissions.Contains("place-bids");
        }));
    options.AddPolicy("ManageProfilePolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permissions").Select(c => c.Value).ToList();
            return permissions.Contains("manage-profile");
        }));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/auth"), appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

// Configure the HTTP request pipeline.
app.UseWebSockets();
app.UseCors("FrontendPolicy");
app.UseRouting();
app.UseAuthorization();
app.MapReverseProxy();


app.Run();
