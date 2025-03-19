
Dans ce travail, J’ai migré une application initialement conçue pour une base de données SQL classique vers une base de données NoSQL Cosmos DB. Cette migration permet de bénéficier de la flexibilité et des performances offertes par Cosmos DB tout en testant localement à l'aide de l'émulateur Azure Cosmos DB emulator. Le but principal est de garantir que toutes les fonctionnalités (création de posts, gestion des commentaires, likes/dislikes) fonctionnent correctement avec cette nouvelle architecture de base de données.Ce processus a impliqué plusieurs étapes, depuis la reconfiguration du contexte de base de données jusqu'à l'adaptation du code pour gérer les entités avec Cosmos DB


Ce que j'ai fait
1. Configuration de la base de données Cosmos DB
J'ai modifié le fichier ApplicationDbContext pour permettre à l'application de se connecter à Cosmos DB. Cela a été fait à l'aide de la méthode UseCosmos. Cette méthode configure le projet pour qu'il utilise Cosmos DB comme base de données principale. Chaque entité (comme les posts et les commentaires) a été associée à un conteneur spécifique dans Cosmos DB, ce qui est essentiel pour organiser les données dans une base NoSQL.
2. Mise à jour des entités
J’ai modifié Post et Comment pour qu'ils fonctionnent correctement avec Cosmos DB :
* Chaque entité a maintenant un champ Id de type GUID, nécessaire pour Cosmos DB.
* J'ai défini un champ PartitionKey pour garantir une gestion efficace des données dans un environnement distribué.
* Certaines propriétés qui n'étaient pas critiques pour le fonctionnement (comme IsDeleted ou IsApproved) ont été mises en commentaire pour simplifier la structure et éviter les conflits inutiles avec Cosmos DB.
3. Adaptation des contrôleurs
Les contrôleurs PostsController et CommentsController ont été entièrement adaptés pour gérer les données en fonction des contraintes de Cosmos DB :
* Les méthodes permettant de créer, lire, liker et disliker les posts et les commentaires ont été modifiées pour utiliser des GUID.
* J'ai également revu les requêtes pour récupérer et afficher les données associées (comme les commentaires liés à un post) en respectant les spécificités de Cosmos DB.
4. Configuration de l'application et connexion à l'émulateur
Pour connecter l'application à Cosmos DB, j'ai utilisé l'émulateur Azure Cosmos DB local. Cela m'a permis de tester toutes les fonctionnalités. La connectionString nécessaire pour l'émulateur a été obtenue directement depuis son interface.
Voici un extrait du fichier appsettings.json où la ConnectionString a été configurée:
"ConnectionStrings": {
  "DefaultConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;",
  "databaseName": "CosmosApplicationDB"
}. La connectionString provient de l'interface de l'émulateur Cosmos DB local. J'ai copié la Primary Connection String, qui est utilisée pour établir la connexion
Cela permet à l'application de se connecter au simulateur local de Cosmos DB et de voir tous les commentaires, posts, .. qui ont été faits dans le site.


5. Gestion des vues et fonctionnalités
J'ai aussi ajusté les vues pour que les données affichées soient bien extraites de Cosmos DB. Les boutons pour liker, dislike, et commenter fonctionnent désormais avec des GUID comme identifiants uniques.
Conclusion
Avec cette migration, j'ai réussi à faire fonctionner toutes les fonctionnalités initiales de l'application sur Cosmos DB via l'émulateur local. Cela garantit que l'application est prête à être déployée dans un environnement Azure en remplaçant simplement la ConnectionString locale par celle fournie par le portail Azure. Si des modifications ou des ajouts futurs sont nécessaires, cette base est prête à évoluer.
