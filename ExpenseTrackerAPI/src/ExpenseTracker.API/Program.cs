using ExpenseTracker.Service.Services;
using ExpenseTracker.Service.Services.Auth;
using ExpenseTracker.Repository.Repositories;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using ExpenseTracker.Service.Services.Auth.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add CORS policy to allow requests from other origins (development friendly)
var frontendOrigins = builder.Configuration["FRONTEND_ORIGINS"] ?? "http://localhost:3000,http://localhost:5000,http://localhost:5001,http://frontend:80";
var origins = frontendOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader();
              // Do not call AllowCredentials() here unless you explicitly need cross-origin cookies
    });
});

// Register FluentValidation validators explicitly (no AspNetCore extension required)
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.SocialLoginRequest>, SocialLoginRequestValidator>();
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.RefreshRequest>, RefreshRequestValidator>();
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.ForgotPasswordRequest>, ForgotPasswordRequestValidator>();
builder.Services.AddScoped<IValidator<ExpenseTracker.Dtos.Auth.ResetPasswordRequest>, ResetPasswordRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth options and JWT options registration
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwtOptions = new JwtOptions { Secret = builder.Configuration["JWT_SECRET"] ?? "replace_this_secret" };
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<ITokenService>(sp => sp.GetRequiredService<JwtTokenService>());

builder.Services.AddSingleton<AuthOptions>(new AuthOptions());
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };
});

// NHibernate configuration (very minimal - replace with proper connection settings)
var cfg = new Configuration();
string usedConnectionString = builder.Configuration["CONNECTION_STRING"] ?? "Host=localhost;Port=5432;Database=postgres;Username=hasnainbukhari;Password=;";
cfg.DataBaseIntegration(db =>
{
    db.ConnectionString = usedConnectionString;
    db.Dialect<PostgreSQL82Dialect>();
    db.Driver<NpgsqlDriver>();
    // Enable SQL logging to console to capture executed SQL and parameters
    db.LogSqlInConsole = true;
    db.LogFormattedSql = true;
    db.AutoCommentSql = true;
});

// load mappings from the repository assembly so embedded resources resolve correctly
cfg.AddResource("ExpenseTracker.Repository.Mapping.AuthMappings.hbm.xml", typeof(NativeUserRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.Entities.hbm.xml", typeof(NativeUserRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.CategoryMappings.hbm.xml", typeof(NativeCategoryRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.CurrencyMappings.hbm.xml", typeof(NativeUserRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.AccountTypeMappings.hbm.xml", typeof(NativeUserRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.AccountMappings.hbm.xml", typeof(NativeUserRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.TransactionMappings.hbm.xml", typeof(NativeTransactionRepository).Assembly);

ISessionFactory? sessionFactory = null;
try
{
    sessionFactory = cfg.BuildSessionFactory();
    builder.Services.AddSingleton<ISessionFactory>(sessionFactory);
    Console.WriteLine("INFO: NHibernate SessionFactory built successfully.");
    Console.WriteLine($"INFO: Using connection string: {usedConnectionString}");
}
catch (Exception ex)
{
    // Log error and rethrow so the container fails fast; NHibernate mapping issues should be fixed rather than hidden
    Console.WriteLine("ERROR: Failed to build NHibernate SessionFactory: " + ex);
    throw;
}

// Register repositories (NHibernate-backed implementations). If BuildSessionFactory failed the app will not reach here.
builder.Services.AddScoped<IUserRepository, NativeUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, NativeRefreshTokenRepository>();
builder.Services.AddScoped<IPasswordResetRepository, NativePasswordResetRepository>();
// Register category repository/service
builder.Services.AddScoped<ICategoryRepository, NativeCategoryRepository>();
builder.Services.AddScoped<CategoryService>();

// Currency & AccountType repositories
builder.Services.AddScoped<ICurrencyRepository, NativeCurrencyRepository>();
builder.Services.AddScoped<IAccountTypeRepository, NativeAccountTypeRepository>();
// Account repository placeholder (implement later)
builder.Services.AddScoped<IAccountRepository, NativeAccountRepository>();
builder.Services.AddScoped<AccountService>();
// Transaction repository and service
builder.Services.AddScoped<ITransactionRepository, NativeTransactionRepository>();
builder.Services.AddScoped<TransactionService>();
// Register services
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<AccountTypeService>();
// Service-level implementations to be added in ExpenseTracker.Service project if needed

// Register auth service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ProfileService>();

var app = builder.Build();

// Run a simple SQL script or ensure DB connectivity on startup (placeholder)
using (var scope = app.Services.CreateScope())
{
    var sf = scope.ServiceProvider.GetService<ISessionFactory>();
    if (sf != null)
    {
        // In production run proper migrations; here we just test opening a session.
        using var session = sf.OpenSession();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Enable CORS middleware using the policy defined above
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
