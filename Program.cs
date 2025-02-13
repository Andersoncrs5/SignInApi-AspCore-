using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SignInApi.Context;
using SignInApi.SetRepositories.InterfacesRepositories;
using SignInApi.SetRepositories.Repositories;
using SignInApi.SetUnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API for system of sign-in with Swagger and PostgreSQL",
        Contact = new OpenApiContact
        {
            Name = "Anderson",
            Email = "anderson.c.rms2005@gmail.com"
        }
    });
});

builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyHeader()   
              .AllowAnyMethod();  
    });
});

//UnitOfWork
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();