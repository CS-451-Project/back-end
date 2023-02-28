using FluentValidation;
using FluentValidation.AspNetCore;
using GivingCircle.Api.Authorization;
using GivingCircle.Api.DataAccess;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.DataAccess.Repositories;
using GivingCircle.Api.Providers;
using GivingCircle.Api.Requests;
using GivingCircle.Api.Requests.FundraiserService;
using GivingCircle.Api.Validation;
using GivingCircle.Api.Validation.FundraiserService;
using Microsoft.AspNetCore.Authentication;
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

    // Register authentication handler
    services.AddAuthentication("BasicAuthentication")
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

    // Register repositories
    var postgresClient = new PostgresClient(new PostgresClientConfiguration()
    {
        ConnectionString = builder.Configuration.GetConnectionString("DbConnection")
    });
    services.AddSingleton<IFundraiserRepository>(x => new FundraiserRepository(postgresClient));
    services.AddSingleton<IBankAccountRepository>(x => new BankAccountRepository(postgresClient));
    services.AddSingleton<IUserRepository>(x => new UserRepository(postgresClient));
    services.AddSingleton<IIdentityRoleRepository>(x => new IdentityRoleRepository(postgresClient));

    // Register Providers
    services.AddSingleton<IIdentityRoleProvider, IdentityRoleProvider>();
    services.AddSingleton<IUserProvider, UserProvider>();

    // Register automatic fluent validation
    services.AddFluentValidationAutoValidation();

    // Register validators
    services.AddSingleton<IValidator<CreateFundraiserRequest>, CreateFundraiserRequestValidator>();
    services.AddSingleton<IValidator<AddBankAccountRequest>, AddBankAccountRequestValidator>();
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