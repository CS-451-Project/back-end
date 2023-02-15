using FluentValidation;
using FluentValidation.AspNetCore;
using GivingCircle.Api.DataAccess;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using GivingCircle.Api.Services;
using GivingCircle.Api.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the dependency injection container
{
    var services = builder.Services;

    // Add controllers
    services.AddControllers();

    // Add http context accessor access to classes
    services.AddHttpContextAccessor();

    // Register repositories
    services.AddSingleton<IFundraiserRepository>(x => new FundraiserRepository(
        new PostgresClient(
            new PostgresClientConfiguration
            { 
                ConnectionString = builder.Configuration.GetConnectionString("DbConnection")
            }
            )));

    // Register services
    services.AddSingleton<IFundraiserService, FundraiserService>();

    // Register automatic fluent validation
    services.AddFluentValidationAutoValidation();

    // Register validators
    services.AddSingleton<IValidator<Fundraiser>, FundraiserValidator>();
}

var app = builder.Build();

// Configure application
{
    app.UseHttpsRedirection();

    // Enable CORS
    app.UseCors(options =>
    {
    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

    // Enable authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Enable controllers
    app.MapControllers();
}

app.Run();

// Make "program.cs" file available for integration testing
public partial class Program { }