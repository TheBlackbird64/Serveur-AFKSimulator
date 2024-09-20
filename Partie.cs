

using System.Diagnostics;

public class Partie
{
    public static List<Partie> parties = new List<Partie>();
    public static List<Joueur> fileAttente = new List<Joueur>();
    public static int nbJoueursMin = 5;
    public static int nbJoueursMax = 10;
    public static int actualiserIntervalleMs = 50;

    public List<Joueur> listeJoueurs { get; set; }
    public List<Projectile> listeProjectile { get; set; }
    public int nbJoueurs { get { return listeJoueurs.Count; } }
    public int graine { get; set; }
    public bool started { get; set; }
    public Stopwatch chrono;



    public static void ActualiserParties()
    {
        // Créer une partie si la file d'attente est longue
        if (fileAttente.Count >= nbJoueursMin)
        {
            Partie p = new Partie();
            parties.Add(p);
        }

        // Remplit les parties en cours
        foreach (Partie p in parties)
        {
            while ((p.nbJoueurs < nbJoueursMax) && (fileAttente.Count > 0))
            {
                int lastPos = fileAttente.Count - 1;
                p.listeJoueurs.Add(fileAttente[lastPos]);
                fileAttente.RemoveAt(lastPos);
            }

            if ((p.listeJoueurs.Count >= nbJoueursMin) && ! p.started)
            {
                p.Start();
            }
        }

        // Actualise les parties en cours
        foreach (Partie p in parties)
        {
            p.Actualiser();
        }
    }


    public Partie()
    {
        listeJoueurs = new List<Joueur>();
        Random rnd = new Random();
        graine = rnd.Next(1000, 9999);
        chrono = new Stopwatch();
        chrono.Start();
    }

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

    public void Actualiser()
    {
        // Actualisation de toute la map (positions, vies, ..)
        if (chrono.ElapsedMilliseconds > actualiserIntervalleMs)
        {
            chrono.Stop();
            chrono.Reset();

            String infosJoueurs = "";
            foreach (Joueur c in listeJoueurs)
            {
                infosJoueurs += string.Join(Joueur.sep4, [c.id.ToString(), c.pseudo, c.x.ToString(), c.y.ToString(), c.vie.ToString(), c.couleur.ToString()]) + Joueur.sep3;
            }

            foreach (Joueur c in listeJoueurs)
            {


                c.EnvoyerMessage(string.Join(Joueur.sep2, ["a", c.tempsAfkMs, infosJoueurs])); // Message d'actualisation pour synchroniser les clients
            }
            
            chrono.Start();
        }
    }
}