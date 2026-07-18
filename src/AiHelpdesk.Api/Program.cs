using System.Text;
using AiHelpdesk.Core.Interfaces.Repositories;
using AiHelpdesk.Core.Interfaces.Services;
using AiHelpdesk.Infrastructure.Identity;
using AiHelpdesk.Infrastructure.Persistence;
using AiHelpdesk.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// TODO Fase 1 (prossimo item): sostituire con un provider basato sui claim JWT della richiesta corrente.
builder.Services.AddScoped<ICurrentTenantProvider, NullCurrentTenantProvider>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddSignInManager()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Sezione di configurazione 'Jwt' mancante.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "AiHelpdesk API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint dev-only per creare/autenticare manualmente un utente reale prima che esistano
// i veri endpoint di login/registrazione (Fase 2). Da rimuovere quando quelli saranno pronti.
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/dev/register", async (
        DevRegisterRequest request,
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator) =>
    {
        var appUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = request.Email, Email = request.Email };
        var createResult = await userManager.CreateAsync(appUser, request.Password);
        if (!createResult.Succeeded)
        {
            return Results.BadRequest(createResult.Errors.Select(e => e.Description));
        }

        var domainUser = new AiHelpdesk.Core.Entities.User
        {
            Id = appUser.Id,
            TenantId = request.TenantId,
            Email = request.Email,
            DisplayName = request.DisplayName,
            Role = request.Role,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await userRepository.AddAsync(domainUser);

        return Results.Ok(new { token = tokenGenerator.GenerateToken(domainUser) });
    });

    app.MapPost("/api/dev/login", async (
        DevLoginRequest request,
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator) =>
    {
        var appUser = await userManager.FindByEmailAsync(request.Email);
        if (appUser is null || !await userManager.CheckPasswordAsync(appUser, request.Password))
        {
            return Results.Unauthorized();
        }

        var domainUser = await userRepository.GetByIdAsync(appUser.Id);
        if (domainUser is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new { token = tokenGenerator.GenerateToken(domainUser) });
    });
}

app.Run();

internal sealed record DevRegisterRequest(
    string Email,
    string Password,
    Guid? TenantId,
    AiHelpdesk.Core.Enums.UserRole Role,
    string DisplayName);

internal sealed record DevLoginRequest(string Email, string Password);

/// <summary>Aggiunge lo schema di sicurezza Bearer al documento OpenAPI, per poter testare gli endpoint [Authorize] dalla Swagger UI.</summary>
internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (schemes.All(s => s.Name != JwtBearerDefaults.AuthenticationScheme))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
        };

        var bearerReference = new OpenApiSecuritySchemeReference("Bearer", document);

        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations!.Values))
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement { [bearerReference] = [] });
        }
    }
}
