using GestionCurriculum.Components;
using GestionCurriculum.Components.Account;
using GestionCurriculum.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;


using Datos.Repositorios;
using Negocios.Interfaces;
using Negocios.Implementaciones;
using Servicios.Interfaces;
using Servicios.Implementaciones;
using Servicios.MapProfiles;
using Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// --------------------------------------------
// Registro de repositorio genérico
// --------------------------------------------
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// --------------------------------------------
// Registro de servicios por entidad
// --------------------------------------------
builder.Services.AddScoped<IProfesorServicio, ProfesorServicio>();
builder.Services.AddScoped<ICongresoServicio, CongresoServicio>();
builder.Services.AddScoped<IEstanciaServicio, EstanciaServicio>();
builder.Services.AddScoped<IProyectoInvestigacionServicio, ProyectoInvestigacionServicio>();
builder.Services.AddScoped<IPublicacionServicio, PublicacionServicio>();
builder.Services.AddScoped<IHabilidadServicio, HabilidadServicio>();
builder.Services.AddScoped<IExperienciaServicio, ExperienciaServicio>();
builder.Services.AddScoped<IFormacionServicio, FormacionServicio>();
builder.Services.AddScoped<ICertificacionServicio, CertificacionServicio>();
builder.Services.AddScoped<IParticipacionAcademicaServicio, ParticipacionAcademicaServicio>();
builder.Services.AddScoped<IDocumentoServicio, DocumentoServicio>();
builder.Services.AddScoped<IProfesorNegocio, ProfesorNegocio>();

// --------------------------------------------
// AutoMapper (todos los perfiles en la ensambladura actual)
// --------------------------------------------
builder.Services.AddAutoMapper(typeof(ProfesorProfile).Assembly);

// --------------------------------------------
// Construcción de la app
// --------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints de identidad de Razor
app.MapAdditionalIdentityEndpoints();

app.Run();
