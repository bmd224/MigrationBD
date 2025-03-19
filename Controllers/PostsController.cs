using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using WebApplicationtp1.Models;

namespace WebApplicationtp1.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructeur pour le contexte de la bd
        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts - Affiche la liste des posts
        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _context.Posts.ToListAsync();

                // Charger les commentaires pour chaque post 
                foreach (var post in posts)
                {
                    post.Comments = await _context.Comments
                        .Where(c => c.PostId == post.Id)
                        .ToListAsync();
                }

                return View(posts);
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
                return View(new List<Post>());
            }
        }

        // GET: Posts/Details/{id} - Affiche les details d un post
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
                if (post == null) return NotFound();

                return View(post);
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
                return NotFound();
            }
        }

        // GET: Posts/Create - Affiche le formulaire pour creer un nouveau post
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create - Enregistre un nouveau post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Category,User,FileToUpload")] PostForm postForm)
        {
            if (!ModelState.IsValid) return View(postForm);

            try
            {
                if (postForm.FileToUpload == null || postForm.FileToUpload.Length == 0)
                {
                    ModelState.AddModelError("FileToUpload", "Veuillez télécharger une image.");
                    return View(postForm);
                }

                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await postForm.FileToUpload.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = postForm.Title,
                    Category = postForm.Category,
                    User = postForm.User,
                    Image = imageData,
                    Created = DateTime.UtcNow
                };

                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
                ModelState.AddModelError("", "Une erreur est survenue lors de l'ajout du post.");
                return View(postForm);
            }
        }

        // POST: Posts/Like - Ajoute un like a un post
        public async Task<IActionResult> Like(Guid postId)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null)
                {
                    post.IncrementLike();
                    await _context.SaveChangesAsync();
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Posts/Dislike - pour ajouter un dislike a un post
        public async Task<IActionResult> Dislike(Guid postId)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post != null)
                {
                    post.IncrementDislike();
                    await _context.SaveChangesAsync();
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        // Méthode pour convertir un fichier en tableau de bytes
        private async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0) ;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

    }
}


