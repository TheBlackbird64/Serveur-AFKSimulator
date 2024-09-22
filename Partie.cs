

using System.Diagnostics;

public class Partie
{
    public static List<Partie> parties = new List<Partie>();
    public static List<Joueur> fileAttente = new List<Joueur>();
    public static int nbJoueursMin = 5;
    public static int nbJoueursMax = 10;
    public static int actualiserIntervalleMs = 50;

    public object lockObj { get; set; } = new object();
    public List<Joueur> listeJoueurs { get; set; }
    public List<Projectile> listeProjectile { get; set; }
    public int nbJoueurs { get { return listeJoueurs.Count; } }
    public int graine { get; set; }
    public bool started { get; set; }


    // Boucle asynchrone qui place les joueurs de la file d'attente dans les parties incompletes, crée les parties si nécéssaire et actualise les parties existantes.
    public static async void ActualiserPartiesAsync()
    {
        while (true)
        {
            await Task.Delay(actualiserIntervalleMs);

            // Supprimer les joueurs déconnectés file d'attente
            foreach (Joueur j in new List<Joueur>(fileAttente))
            {
                if (j.suppr) { Partie.fileAttente.Remove(j); }
            }
            

            // Créer une partie si la file d'attente est longue
            if (fileAttente.Count >= nbJoueursMin)
            {
                Partie p = new Partie();
                parties.Add(p);
            }
            
            foreach (Partie p in parties)
            {
                // Actualise chaque parties
                if (p.started)
                {
                    p.Actualiser();
                }

                // Remplit les parties en cours
                while ((p.nbJoueurs < nbJoueursMax) && (fileAttente.Count > 0))
                {
                    p.listeJoueurs.Add(fileAttente[0]);
                    fileAttente.RemoveAt(0);
                }
                // Demarre les parties remplies
                if ((p.listeJoueurs.Count >= nbJoueursMin) && !p.started)
                {
                    p.Start();
                }

                // Supprimer les joueurs déconnectés dans les parties
                foreach (Joueur j in new List<Joueur> (p.listeJoueurs))
                {
                    if (j.suppr) { p.listeJoueurs.Remove(j); }
                }
            }
        }
    }


    public Partie()
    {
        listeJoueurs = new List<Joueur>();
        listeProjectile = new List<Projectile>();

        Random rnd = new Random();
        graine = rnd.Next(1000, 9999);
    }

    // Commence une partie, prévient les clients du début de la partie
    public void Start()
    {
        started = true;
        foreach (Joueur client in listeJoueurs)
        {
            client.partie = this;
            client.EnvoyerMessage(string.Join(Joueur.sep2, ["p", graine.ToString(), client.id])); // Message de lancement de la partie
        }

        Console.WriteLine("-- Partie démarrée (" + parties.Count + " parties en cours)");
    }

    // Actualisation de toute la map (positions, vies, ..)
    public void Actualiser()
    {
        // Actualisation des ElementMap 
        // Pour optimiser, l'actualisation des objets se fait dans la même boucle que celle qui sert à récupérer les infos pour l'actualisation

        string infosProjectiles = "";
        foreach (Projectile p in listeProjectile)
        {
            p.Actualiser();
            infosProjectiles += string.Join(Joueur.sep4, [p.id.ToString(), p.x.ToString(), p.y.ToString(), p.direction.ToString()]) + Joueur.sep3;
        }

        // Envoi du message d'actualisation pour les clients : Construction du message
        string infosJoueurs = "";
        foreach (Joueur j in listeJoueurs)
        {
            j.Actualiser(); // Actualisation
            infosJoueurs += string.Join(Joueur.sep4, [j.id.ToString(), j.pseudo, j.x.ToString(), j.y.ToString(), j.vie.ToString(), j.couleur.ToString()]) + Joueur.sep3;
        }

        // Envoi
        lock (lockObj)
        {
            foreach (Joueur j in listeJoueurs)
            {
                j.EnvoyerMessage(string.Join(Joueur.sep2, ["a", j.tempsAfkMs, infosJoueurs, infosProjectiles])); // Message d'actualisation pour synchroniser les clients
            }
        }
    }
}