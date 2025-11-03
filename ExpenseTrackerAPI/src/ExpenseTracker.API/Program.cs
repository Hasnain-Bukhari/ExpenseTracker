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
// In development, allow requests from any origin on the local network
var isDevelopment = builder.Environment.IsDevelopment();
var frontendOrigins = builder.Configuration["FRONTEND_ORIGINS"] 
    ?? (isDevelopment 
        ? "http://localhost:3000,http://localhost:3001,http://localhost:3002,http://localhost:3003,http://localhost:3004,http://localhost:5001,http://frontend:80" 
        : "http://localhost:3000,http://localhost:5001,http://frontend:80");

// In development, also allow any IP on the local network (192.168.x.x, 10.x.x.x, 172.16-31.x.x)
if (isDevelopment)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy
                .SetIsOriginAllowed(origin =>
                {
                    // Allow localhost with any port
                    if (origin.StartsWith("http://localhost:") || origin.StartsWith("https://localhost:"))
                        return true;
                    
                    // Allow any IP on local network (192.168.x.x, 10.x.x.x, 172.16-31.x.x)
                    var uri = new Uri(origin);
                    var host = uri.Host;
                    var ipParts = host.Split('.');
                    if (ipParts.Length == 4)
                    {
                        var firstOctet = int.Parse(ipParts[0]);
                        var secondOctet = int.Parse(ipParts[1]);
                        
                        // 192.168.x.x
                        if (firstOctet == 192 && secondOctet == 168)
                            return true;
                        
                        // 10.x.x.x
                        if (firstOctet == 10)
                            return true;
                        
                        // 172.16.x.x - 172.31.x.x
                        if (firstOctet == 172 && secondOctet >= 16 && secondOctet <= 31)
                            return true;
                    }
                    
                    return false;
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}
else
{
    // Production: only allow specific origins
    var origins = frontendOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

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
builder.Services.AddScoped<IEmailService, GmailEmailService>();

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
// Read connection string from appsettings: try ConnectionStrings:DefaultConnection first, then CONNECTION_STRING, then fallback
string usedConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? builder.Configuration["CONNECTION_STRING"] 
    ?? "Host=localhost;Port=5432;Database=postgres;Username=hasnainbukhari;Password=;";
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
cfg.AddResource("ExpenseTracker.Repository.Mapping.BudgetMappings.hbm.xml", typeof(NativeBudgetRepository).Assembly);
cfg.AddResource("ExpenseTracker.Repository.Mapping.GoalMappings.hbm.xml", typeof(NativeGoalRepository).Assembly);

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
// Budget repository and service
builder.Services.AddScoped<IBudgetRepository, NativeBudgetRepository>();
builder.Services.AddScoped<BudgetService>();

// Goal services
builder.Services.AddScoped<IGoalRepository, NativeGoalRepository>();
builder.Services.AddScoped<IGoalService, GoalService>();
// Register services
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<AccountTypeService>();
// Service-level implementations to be added in ExpenseTracker.Service project if needed

// Register auth service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ProfileService>();

// Register dashboard service
builder.Services.AddScoped<DashboardService>();

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
