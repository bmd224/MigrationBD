using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using WebApplicationtp1.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur
builder.Services.AddControllersWithViews();

// Configuration de CosmosDB pour le DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseCosmos(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        databaseName: "CosmosApplicationDB",
        cosmosOptionsAction: cosmosOptions =>
        {
            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
            cosmosOptions.MaxRequestsPerTcpConnection(16);
            cosmosOptions.MaxTcpConnectionsPerEndpoint(32);
        })
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableDetailedErrors() // erreurs
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Initialisation des données CosmosDB
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Cree les conteneurs dans CosmosDB 
        await context.Database.EnsureCreatedAsync();

        // Ajouter des donnees de test si le conteneur est vide
        if (!await context.Posts.AnyAsync())
        {
            context.Posts.Add(new Post
            {
                Id = Guid.NewGuid(),
                Title = "Test Post",
                Category = Category.Humour,
                User = "TestUser",
                Created = DateTime.UtcNow,
                Image = new byte[0] // Ajoutez une image
            });

            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de l'initialisation de la base CosmosDB : {ex.Message}");
    }
}

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
