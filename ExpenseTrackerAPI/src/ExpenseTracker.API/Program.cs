using ExpenseTracker.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ...existing code...

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// ...existing code...

app.Run();
