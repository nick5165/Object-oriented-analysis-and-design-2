using SalesSystem.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<StorageService>();
builder.Services.AddTransient<DocumentGenerator>();

// ЛОГИКА ПЕРЕКЛЮЧЕНИЯ
// Читаем флаг из appsettings.json. Если его нет, по умолчанию true.
bool usePattern = builder.Configuration.GetValue<bool>("UseAbstractFactory", true);

if (usePattern)
{
    builder.Services.AddScoped<IDocumentProcessor, PatternProcessor>();
}
else
{
    builder.Services.AddScoped<IDocumentProcessor, FlatProcessor>();
}

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => { c.RoutePrefix = string.Empty; c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
app.MapControllers();
app.Run();