using Microsoft.EntityFrameworkCore;
using ProductProvider.Repositories;
using ProductProvider.GraphQL;
using ProductProvider.Interfaces;
using ProductProvider.Models.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging();
// Access Token Validation
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtAccess:Key"])),
            ValidIssuer = builder.Configuration["JwtAccess:Issuer"],
            ValidAudience = builder.Configuration["JwtAccess:Audience"],
            ClockSkew = TimeSpan.Zero, // No clock skew for strict expiration checks
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token; // Set the token from the cookie for validation
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                if (claimsIdentity == null)
                {
                    context.Fail("Token är ogiltigt. Claims identity är null."); // Swedish message
                    return Task.CompletedTask;
                }

                // Validate 'isVerified' claim
                var isVerifiedClaim = claimsIdentity.FindFirst("isVerified");

                if (isVerifiedClaim == null)
                {
                    context.Fail("Token är ogiltigt. Kontot är inte verifiead."); // Swedish message
                    return Task.CompletedTask;
                }

                if (isVerifiedClaim.Value != "true")
                {
                    context.Fail("Token är ogiltigt. Användaren är inte verifierad."); // Swedish message
                    return Task.CompletedTask;
                }

                // Log success after validation (log only non-sensitive data)
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token validerades framgångsrikt för användare {UserName}.", claimsIdentity.Name);

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // Log the error, including stack trace for better diagnostics
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Autentisering misslyckades: {context.Exception.Message}"); // Swedish message
                if (context.Exception.StackTrace != null)
                {
                    logger.LogError(context.Exception.StackTrace);
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB limit
});
builder.Services.Configure<PriceSettings>(builder.Configuration.GetSection("PriceSettings"));


// Register repositories and services
builder.Services.AddScoped<IBusinessTypeService, BusinessTypeService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IPriceSettingsService, PriceSettingsService>();

// Set up DbContext for ProductDbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDatabase")));

// Set up DbContext for ReservationDbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDatabase")));

// Add GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<ProductQuery>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");  // Apply the CORS policy globally

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
