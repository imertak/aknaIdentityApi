using aknaIdentity_api.Infrastructure.Extensions;
using akna_api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework Core - ApplicationDbContext'i kaydet
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity services'leri ekle
builder.Services.AddIdentityServices();

// JWT Authentication'ý ekle
builder.Services.AddJwtAuthentication(builder.Configuration);

// Background services'leri ekle
builder.Services.AddCleanupBackgroundService();

var app = builder.Build();

// Otomatik veritabaný oluþturma (Development ortamýnda)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Veritabaný kontrol ediliyor...");

        // Veritabanýný sil ve yeniden oluþtur (sadece development için)
        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Development ortamýnda - Veritabaný siliniyor...");
            await context.Database.EnsureDeletedAsync();

            logger.LogInformation("Veritabaný yeniden oluþturuluyor...");
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Veritabaný ve tablolar baþarýyla oluþturuldu!");
        }
        else
        {
            // Production'da sadece oluþtur
            await context.Database.EnsureCreatedAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabaný oluþturulurken hata oluþtu: {Error}", ex.Message);
        throw; // Uygulamanýn baþlamasýný engelle
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Authentication middleware'i ekle
app.UseAuthorization();

app.MapControllers();

app.Run();