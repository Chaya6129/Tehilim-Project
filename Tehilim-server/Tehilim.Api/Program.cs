using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Text;
using Tehilim.API.Middleware;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Interfaces.Services;
using Tehilim.Data;
using Tehilim.Data.Context;
using Tehilim.Data.Repositories;
using Tehilim.Service;
using Tehilim.Service.Profiles;
using Tehilim.Service.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new ErrorResponse(
            StatusCodes.Status400BadRequest,
            "הבקשה אינה תקינה",
            context.HttpContext.TraceIdentifier,
            errors));
    });// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Tehilim.API.xml"));

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "הדבק את ה-token בלבד, בלי המילה Bearer"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
}); builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// AutoMapper: סורק את הפרויקט של BookProfile ומוצא את כל ה-Profiles בו
builder.Services.AddAutoMapper(typeof(BookProfile));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });

builder.Services.AddAuthorization();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("Location", CorrelationIdMiddleware.HeaderName)));
var clientUrl = builder.Configuration["ClientUrl"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
    options.AddPolicy("Client", policy =>
        policy.WithOrigins(clientUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
