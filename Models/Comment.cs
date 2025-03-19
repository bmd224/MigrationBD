using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationtp1.Models
{
    public class Comment
    {
        // Proprietes principales
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "SVP entrer votre commentaire")]
        [MaxLength(128)]
        public required string Commentaire { get; init; }

        [Required(ErrorMessage = "SVP entrer un nom d'utilisateur")]
        [MaxLength(128)]
        public required string User { get; init; }

        [Display(Name = "Like")]
        public int Like { get; set; } = 0;

        [Display(Name = "Dislike")]
        public int Dislike { get; set; } = 0;

        public DateTime Created { get; init; } = DateTime.UtcNow;

        public Post? Post { get; set; }

        [ForeignKey("PostId")]
        public Comment? ParentComment { get; set; }

        public ICollection<Comment> SubComments { get; private set; } = new List<Comment>();

        // Méthodes pour manipuler les likes et dislikes
        public void IncrementLike() => Like++;
        public void IncrementDislike() => Dislike++;
       
    }
}