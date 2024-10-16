

using Microsoft.VisualBasic;
using System.Diagnostics;

public class Partie
{
    public static List<Partie> listePartie = new List<Partie>();
    public static List<Joueur> fileAttente = new List<Joueur>();
    public const int nbJoueursMin = 1;
    public const int nbJoueursMax = 10;
    public const int actualiserIntervalleMs = 50;
    public const int tempsVictoire = 120;
    public const int tempsSupprPartie = 5;

    public object lockObj { get; set; } = new object();
    public List<Joueur> listeJoueurs { get; set; }
    public List<Projectile> listeProjectile { get; set; }
    public int nbJoueurs { get { return listeJoueurs.Count; } }
    public int graine { get; set; }
    public bool started { get; set; }
    public Stopwatch chrono { get; set; }
    public Map map { get; set; }


    // Boucle asynchrone qui place les joueurs de la file d'attente dans les parties incompletes, crée les parties si nécéssaire et actualise les parties existantes.
    public static async void ActualiserPartiesAsync()
    {
        while (true)
        {
            await Task.Delay(actualiserIntervalleMs);

            SupprimerElementsMap<Joueur> (fileAttente);

            foreach (Partie p in new List<Partie> (listePartie))
            {
                // Actualise chaque parties
                if (p.started)
                {
                    p.Actualiser();
                }

                // Remplit les parties en cours
                while ((p.nbJoueurs < nbJoueursMax) && (fileAttente.Count > 0))
                {
                    Joueur j = fileAttente[0];
                    p.listeJoueurs.Add(j);
                    fileAttente.RemoveAt(0);

                    if (p.started) { p.StartClient(j); }
                }

                // Demarre les parties remplies
                if ((p.listeJoueurs.Count >= nbJoueursMin) && !p.started)
                {
                    p.started = true;
                    Console.WriteLine("-- Partie démarrée (" + listePartie.Count + " parties en cours)");

                    foreach (Joueur j in p.listeJoueurs)
                    {
                        p.StartClient(j);
                    }
                }

                SupprimerElementsMap<Joueur>(p.listeJoueurs);
            }

            // Créer une partie si la file d'attente est longue
            if (fileAttente.Count >= nbJoueursMin)
            {
                Partie p = new Partie();
                listePartie.Add(p);
            }
        }
    }

    // Supprimer les éléments qui ne servent plus dans une liste d'éléments de la map
    public static void SupprimerElementsMap<T> (List<T> Elem) where T : ElementMap
    {
        foreach (T e in new List<T> (Elem))
        {
            if (e.suppr) { Elem.Remove(e); }
        }
    }


    public Partie()
    {
        Console.WriteLine("Partie créée");

        listeJoueurs = new List<Joueur>();
        listeProjectile = new List<Projectile>();
        chrono = new Stopwatch();

        Random rnd = new Random();
        graine = rnd.Next(1, 1000000);
        map = new Map(graine);

        map.GenTab();
        map.tabObstacle = map.GenLisserTab(map.tabObstacle, 2);
        /*
        // debug 
        for (int i = 0; Map.tailleMap > i; i++)
        {
            for (int j = 0; Map.tailleMap > j; j++)
            {
                if (map.tabObstacle[i, j] > Map.valInf)
                {
                    Console.WriteLine(i.ToString() + "   " + j.ToString() + " : " + map.tabObstacle[i, j].ToString());
                }
            }
        }//*/
    }
   

    // Commence une partie, prévient les clients du début de la partie
    public void StartClient(Joueur j)
    {
        j.partie = this;
        j.EnvoyerMessage(string.Join(Joueur.sep2, ["p", graine.ToString(), j.id]));
    }

    // Actualisation de toute la map (positions, vies, ..)
    public void Actualiser()
    {
        SupprimerElementsMap<Projectile> (listeProjectile);

        // Actualisation des ElementMap 
        // Pour optimiser, l'actualisation des objets se fait dans la même boucle que celle qui sert à récupérer les infos à envoyer aux client pour les actualiser

        string infosProjectiles = "";
        foreach (Projectile p in new List<Projectile> (listeProjectile))
        {
            p.Actualiser();
            infosProjectiles += string.Join(Joueur.sep4, [p.id.ToString(), p.x.ToString(), p.y.ToString(), p.direction.ToString(), p.idJoueur.ToString()]) + Joueur.sep3;
        }

        // Envoi du message d'actualisation pour les clients : Construction du message + vérif si y'en a un qui a gagné
        string infosJoueurs = "";
        Joueur? gagnant = null;
        foreach (Joueur j in listeJoueurs)
        {
            if (j.chrono.Elapsed.TotalSeconds > tempsVictoire) { gagnant = j; break; }
            j.Actualiser();

            infosJoueurs += string.Join(Joueur.sep4, [j.id.ToString(), j.pseudo, j.x.ToString(), j.y.ToString(), j.vie.ToString(), j.couleur.ToString()]) + Joueur.sep3;
        }

        if (gagnant == null)
        {
            // Envoi du message d'actualisation pour les clients
            foreach (Joueur j in listeJoueurs)
            {
                j.EnvoyerMessage(string.Join(Joueur.sep2, ["a", j.tempsAfkMs.ToString(), infosJoueurs, infosProjectiles]));
            }
        }
        else
        {


            // Envoi du message de fin de partie
            foreach (Joueur j in listeJoueurs)
            {
                j.EnvoyerMessage(string.Join(Joueur.sep2, ["g", gagnant.pseudo]));
            }

            Console.WriteLine("Partie terminée (id gagnant : " + gagnant.id.ToString() + " )");
            listePartie.Remove(this);
        }


        // Supprime la partie si elle est inutilisé
        if (!chrono.IsRunning)
        {
            if (listeJoueurs.Count == 0) { chrono.Start(); }
        }
        else
        {
            if (listeJoueurs.Count != 0) { chrono.Stop(); }
            if (chrono.Elapsed.TotalSeconds > tempsSupprPartie)
            {
                listePartie.Remove(this);
                Console.WriteLine("Partie supprimée");
            }
        }
    }
}