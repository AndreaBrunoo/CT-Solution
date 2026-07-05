using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Sln.Domain.Interfaces;
using Sln.DataAccess.Services;

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

// ------------------------------------------------------------
// 2. SERVIZI DELLA TUA APPLICAZIONE
// ------------------------------------------------------------

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------------------------------
// 3. PIPELINE HTTP
// ------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();