using Amazon.S3;
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
        ConnectionString = builder.Configuration.GetConnectionString("DatabaseConnectionString")
    });
    services.AddSingleton<IFundraiserRepository>(new FundraiserRepository(postgresClient));
    services.AddSingleton<IBankAccountRepository>(new BankAccountRepository(postgresClient));
    services.AddSingleton<IUserRepository>(new UserRepository(postgresClient));
    services.AddSingleton<IDonationRepository>(new DonationRepository(postgresClient));

    // Register Providers
    services.AddSingleton<IUserProvider, UserProvider>();
    services.AddSingleton<IFundraiserProvider, FundraiserProvider>();

    // Register AWS S3
    var awsAccessKey = builder.Configuration.GetConnectionString("AwsAccessKeyId");
    var awsSecretKey = builder.Configuration.GetConnectionString("AwsSecretAccessKey");
    services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsAccessKey, awsSecretKey));

    // Register automatic fluent validation
    services.AddFluentValidationAutoValidation();

    // Register validators
    services.AddSingleton<IValidator<CreateFundraiserRequest>, CreateFundraiserRequestValidator>();
    services.AddSingleton<IValidator<AddBankAccountRequest>, AddBankAccountRequestValidator>();
    services.AddSingleton<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
    services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();
    services.AddSingleton<IValidator<MakeDonationRequest>, MakeDonationRequestValidator>();
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