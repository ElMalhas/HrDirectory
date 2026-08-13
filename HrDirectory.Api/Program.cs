using HrDirectory.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connects the Entity Framework to SQLite using a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();          // Tells the app that Controllers will be used

var app = builder.Build();

// Only activate Swagger in DEV environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();                      // Map the routes from the Controllers

// Activate CORS before endpoints
app.UseCors("AllowReactApp");

// Health Check endpoint
app.MapGet("/api/status", () => "API is working!")
   .WithName("GetStatus");

app.Run();