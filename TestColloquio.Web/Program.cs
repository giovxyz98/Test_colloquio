using Test_colloquio;

var builder = WebApplication.CreateBuilder(args);

// Carica config.ini se presente nella cartella dell'applicazione
string iniPath = Path.Combine(AppContext.BaseDirectory, "config.ini");
IniLoader.Load(iniPath);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra Repository usando la connection string da Predefiniti_Database (o da appsettings.json)
string connectionString = builder.Configuration.GetConnectionString("Default")
    ?? Predefiniti_Database.ConnectionString;

builder.Services.AddScoped<Repository>(_ => new Repository(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
