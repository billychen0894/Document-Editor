using CollabDocumentEditor.Core.Authorization;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Core.Validators.AuthValidators;
using CollabDocumentEditor.Infrastructure.Authorization;
using CollabDocumentEditor.Infrastructure.Data;
using CollabDocumentEditor.Infrastructure.Extensions;
using CollabDocumentEditor.Infrastructure.HealthChecks;
using CollabDocumentEditor.Infrastructure.Mapping;
using CollabDocumentEditor.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("No connection string found.");
    
// Add services to the container.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddControllers();

// Registers all validators in ProjectName.Core assembly
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly)
        ));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Signin settings
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthorizationHandler, DocumentOwnerHandler>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    
    options.AddPolicy("DocumentOwner", policy => policy.AddRequirements(new DocumentOwnerRequirement()));
});

builder.Services.AddEmailServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    // app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Run();