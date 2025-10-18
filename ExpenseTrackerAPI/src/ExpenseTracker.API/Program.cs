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

// Add controllers
builder.Services.AddControllers();

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
cfg.DataBaseIntegration(db =>
{
    // Connect to local Postgres as requested: database 'postgress', user 'hasnainbukhari', no password
    db.ConnectionString = "Host=localhost;Port=5432;Database=postgress;Username=hasnainbukhari;";
    db.Dialect<PostgreSQL82Dialect>();
    db.Driver<NpgsqlDriver>();
});

// load mappings from the repository assembly so embedded resources resolve correctly
cfg.AddResource("ExpenseTracker.Repository.Mapping.AuthMappings.hbm.xml", typeof(NativeUserRepository).Assembly);

var sessionFactory = cfg.BuildSessionFactory();
builder.Services.AddSingleton<ISessionFactory>(sessionFactory);

// Register repositories
builder.Services.AddScoped<IUserRepository, NativeUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, NativeRefreshTokenRepository>();
builder.Services.AddScoped<IPasswordResetRepository, NativePasswordResetRepository>();

// Register auth service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Run a simple SQL script or ensure DB connectivity on startup (placeholder)
using (var scope = app.Services.CreateScope())
{
    var sf = scope.ServiceProvider.GetRequiredService<ISessionFactory>();
    // In production run proper migrations; here we just test opening a session.
    using var session = sf.OpenSession();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
