using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationtp1.Models
{
    // Enumération pour les catégories de posts
    public enum Category
    {
        Humour = 0,
        Nouvelle = 1,
        Inconnue = 2
    }

    // Classe non mappee pour recevoir les donnees du formulaire de creation de post
    [NotMapped]
    public class PostForm : Post
    {
        [Required(ErrorMessage = "Un fichier est requis, il doit être de 20 MB et moins")]
        public required IFormFile FileToUpload { get; set; }
    }

    // Classe principale pour les posts
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "SVP entrer un titre")]
        [MaxLength(128)]
        [Display(Name = "Titre")]
        public required string Title { get; init; }

        [Required(ErrorMessage = "SVP entrer une catégorie")]
        [EnumDataType(typeof(Category))]
        [Display(Name = "Catégorie")]
        public Category Category { get; init; }

        [Required(ErrorMessage = "SVP entrer un nom d'utilisateur")]
        [MaxLength(128)]
        [Display(Name = "Nom de l'utilisateur")]
        public required string User { get; init; }

        [Display(Name = "Like")]
        public int Like { get; private set; } = 0;

        [Display(Name = "Dislike")]
        public int Dislike { get; private set; } = 0;

        [Display(Name = "Date de création")]
        public DateTime Created { get; init; } = DateTime.Now;

        
        public byte[]? Image { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Cle pour la base de donnees
        [Required]
        public Guid PartitionKey => Id;

        // Methodes pour manipuler les likes et dislikes
        public void IncrementLike() => Like++;
        public void IncrementDislike() => Dislike++;
    }
}


