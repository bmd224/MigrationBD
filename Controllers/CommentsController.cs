using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using WebApplicationtp1.Models;


namespace WebApplicationtp1.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructeur pour le contexte de la bd
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments - recuperer tous les commentaires d un post specifique
        public async Task<IActionResult> Index(Guid id)
        {
            try
            {
                // Charger les commentaires lies a un post 
                var comments = await _context.Comments
                    .Where(w => w.PostId == id)
                    .ToListAsync();

                return View(comments);
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
                return View(new List<Comment>()); // Retourne une liste vide en cas d'erreur
            }
        }

        // GET: Comments/Create/{PostId} - Affiche le formulaire pour la creation d'un commentaire
        [HttpGet]
        public IActionResult Create(Guid PostId)
        {
            ViewData["PostId"] = PostId;
            return View();
        }

        // POST: Comments/Create - Ajouter un nouveau commentaire
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Commentaire,User,PostId")] Comment comment)
        {
            // Supprimer les erreurs liées aux propriétés de navigation non supportées directement par CosmosDB
            ModelState.Remove("Post");

            if (ModelState.IsValid)
            {
                try
                {
                    // Générer un GUID pour le commentaire
                    comment.Id = Guid.NewGuid();

                    await _context.Comments.AddAsync(comment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = comment.PostId });
                }
                catch (CosmosException ex)
                {
                    Console.WriteLine($"CosmosDB Error: {ex.Message}");
                }
            }

            return RedirectToAction(nameof(Index), new { id = comment.PostId });
        }

        // POST: Comments/Like - Ajouter un like a un commentaire
        public async Task<IActionResult> Like(Guid CommentId, Guid PostId)
        {
            try
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == CommentId);
                if (comment != null)
                {
                    comment.IncrementLike();
                    await _context.SaveChangesAsync();
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
            }

            return RedirectToAction(nameof(Index), new { id = PostId });
        }

        // POST: Comments/Dislike - Ajouter un dislike à un commentaire
        public async Task<IActionResult> Dislike(Guid CommentId, Guid PostId)
        {
            try
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == CommentId);
                if (comment != null)
                {
                    comment.IncrementDislike();
                    await _context.SaveChangesAsync();
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
            }

            return RedirectToAction(nameof(Index), new { id = PostId });
        }
    }
}
