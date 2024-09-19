using App.Core.Entities;
using App.Infrastructure.Data;
using App.Infrastructure.Identity;
using App.Infrastructure.Persistence;
using App.Presentation;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Configure Kestrel to listen on all network interfaces
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5067); // HTTP port
//    options.ListenAnyIP(7237, listenOptions =>
//    {
//        listenOptions.UseHttps(); // HTTPS port
//    });
//});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddPresentation(configuration);








var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var admin = services.GetRequiredService<IdentitySeeder>();
        await admin.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles and users.");
    }
}


app.MapControllers();

app.Run();
