using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;

namespace WebApplicationtp1.Models
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Constructeur pour la configuration et les options du contexte
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // Configuration des options pour CosmosDB
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection")!;
                var databaseName = "CosmosAppDb";

                optionsBuilder.UseCosmos(
                    connectionString: connectionString,
                    databaseName: databaseName,
                    cosmosOptionsAction: cosmosOptions =>
                    {
                        cosmosOptions.ConnectionMode(ConnectionMode.Direct);
                        cosmosOptions.MaxRequestsPerTcpConnection(16);
                        cosmosOptions.MaxTcpConnectionsPerEndpoint(32);
                    })
                .EnableDetailedErrors();

                Console.WriteLine("Configuration CosmosDb avec succès");
            }
        }

        // Configuration du modele pour les entites
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration pour l entite Post
            modelBuilder.Entity<Post>(entity =>
            {
                // Associer l'entité Post au conteneur CosmosDB "Posts"
                entity.ToContainer("Posts")
                      .HasNoDiscriminator()
                      .HasPartitionKey(x => x.Id)
                      .HasKey(x => x.Id);

                // Relation entre un Post et son Commentaire
                entity.HasMany(x => x.Comments)
                      .WithOne(x => x.Post)
                      .HasForeignKey(x => x.PostId);
            });

            // Configuration pour l'entite Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                // Associe l'entite Comment au conteneur CosmosDB "Comments"
                entity.ToContainer("Comments")
                      .HasNoDiscriminator()
                      .HasPartitionKey(x => x.PostId)
                      .HasKey(x => x.Id);

                // Configuration des proprietes specifiques
                entity.Property(x => x.Id)
                      .HasDefaultValueSql("NEWID()");

                entity.Property(x => x.Created)
                      .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.HasAutoscaleThroughput(4000);
        }

        // Définition des DbSet pour les entités
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
    }
}








