using Api.Extensions;
using Api.Middlewares;
using ApplicationCore;
using ApplicationCore.Dtos.User;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Settings;
using ApplicationCore.Sevices;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var service = builder.Services;
var config = ConfigHelper.SetConfings();
var settings = config.Get<Settings>();

// Add services to the container.

service.AddSingleton<ISettings>(settings);

service.AddControllers();
service.AddHttpContextAccessor();
service.AddFluentValidation();
service.AddValidatorsFromAssembly(typeof(Program).Assembly);
service.AddDbContext<DataContext>();
service.AddMediatR(typeof(ApplicationCoreAssembly).Assembly);
service.AddAutoMapper(typeof(ApplicationCoreAssembly).Assembly);
service.AddJwtAuthentication(settings);

service.AddScoped<ErrorHandlingMiddleware>();
service.AddScoped<IAuthenticationService, AuthenticationService>();
service.AddScoped<ICookieService, CookieService>();
service.AddScoped<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>();
service.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
service.AddScoped<IUnitOfWork, UnitOfWork>();

service.AddEndpointsApiExplorer();
service.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200"));

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<DataContext>();
context.Database.Migrate();

app.Run();