using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Sln.Domain.Interfaces;
using Sln.DataAccess.Services;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.DatabaseSeeder;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. CONFIGURAZIONE SQLITE + XPO
// ------------------------------------------------------------

// Crea il file SQLite se non esiste
var connectionString = SQLiteConnectionProvider.GetConnectionString("AuthDatabase.db");

// Crea automaticamente database + schema XPO
XpoDefault.DataLayer = XpoDefault.GetDataLayer(
    connectionString,
    AutoCreateOption.DatabaseAndSchema
);

XpoDefault.Session = null;

// Registra UnitOfWork per ogni richiesta API
builder.Services.AddScoped<UnitOfWork>(sp =>
{
    return new UnitOfWork(XpoDefault.DataLayer);
});
builder.Services.AddScoped<XpoDataContext>();

// ------------------------------------------------------------
// 2. SERVIZI DELLA TUA APPLICAZIONE
// ------------------------------------------------------------

builder.Services.AddScoped<IActionLogger, ActionLogger>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IActionLogger, ActionLogger>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fatturation Test API",
        Version = "v1"
    });

    // 🔥 JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Inserisci il token JWT nel formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = new List<string>()
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5077")   // frontend Blazor
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddAuthorization();

// ------------------------------------------------------------
// 3. PIPELINE HTTP
// ------------------------------------------------------------

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<XpoDataContext>();
    await DatabaseSeeder.SeedAsync(ctx, CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();