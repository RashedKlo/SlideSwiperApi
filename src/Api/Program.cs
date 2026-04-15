using Application;
using Application.Common.Behaviors;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — allow Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
      policy.WithOrigins(
                "http://localhost:4200",
                "https://slideswiperapp-cp92.onrender.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});
// Rate limiting — 30 requests per minute per client
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 30;          // max 30 requests
        opt.Window = TimeSpan.FromMinutes(1); // per 1 minute
        opt.QueueLimit = 0;            // no queuing
    });
    options.RejectionStatusCode = 429; // Too Many Requests
});
// Global validation behavior using MediatR pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

// ── Middleware ─────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();