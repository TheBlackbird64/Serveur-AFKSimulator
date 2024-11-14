

using System.Diagnostics;
using Serveur_AFKSimulator.Items;

namespace Serveur_AFKSimulator
{
    public class Partie
    {
        public static List<Partie> listePartie = new List<Partie>();
        public static List<Client> fileAttente = new List<Client>();
        public const int nbJoueursMin = 3;
        public const int nbJoueursMax = 7;
        public const int actualiserIntervalleMs = 50;
        public const int tempsVictoire = 61;
        public const int tempsSupprPartie = 5;

        // éléments du jeu
        public List<Client> listeClient { get; set; }
        public bool verrouClient { get; set; } = false; // Pour éviter qu'un client soit supprimé pendant que la partie s'actualise, ce qui ferait planter le serveur
        public List<Projectile> listeProjectile { get; set; }
        public Stopwatch chrono { get; set; }
        public Map map { get; set; }

        public int nbJoueurs { get { return listeClient.Count; } }
        public int graine { get; set; }
        public bool suppr = false;
        public bool started { get; set; }
        public Random rnd = new Random();


        // Boucle asynchrone qui place les joueurs de la file d'attente dans les parties incompletes, crée les parties si nécéssaire et actualise les parties existantes.
        public static async void ActualiserPartiesAsync()
        {
            while (true)
            {
                await Task.Delay(actualiserIntervalleMs);

                foreach (Partie p in new List<Partie>(listePartie))
                {
                    if (! p.suppr)
                    {
                        // Actualise chaque parties
                        if (p.started)
                        {
                            p.Actualiser();
                        }

                        // Remplit les parties en cours
                        while ((p.nbJoueurs < nbJoueursMax) && (fileAttente.Count > 0))
                        {
                            Client c = fileAttente[0];
                            p.listeClient.Add(c);
                            fileAttente.RemoveAt(0);

                            if (p.started) { p.StartClient(c); }
                        }

                        // Demarre les parties remplies
                        if ((p.listeClient.Count >= nbJoueursMin) && !p.started)
                        {
                            p.started = true;
                            Console.WriteLine("-- Partie démarrée (" + listePartie.Count + " parties en cours)");

                            foreach (Client c in p.listeClient)
                            {
                                p.StartClient(c);
                            }
                        }
                    }
                }

                // Créer une partie si la file d'attente est longue
                if (fileAttente.Count >= nbJoueursMin)
                {
                    listePartie.Add(new Partie());
                }

                foreach (Client c in fileAttente)
                {
                    c.EnvoyerMessage(string.Join(Client.sep2, ["f", fileAttente.Count.ToString()]));
                }
            }
        }

        // Supprimer les éléments qui ne servent plus dans une liste d'éléments de la map
        public static void SupprimerElementsMap<T>(List<T> Elem) where T : ElementMap
        {
            foreach (T e in new List<T>(Elem))
            {
                if (e.suppr) { Elem.Remove(e); }
            }
        }

        
        public static double ValeurSync(double valPar10Ms) => valPar10Ms * (actualiserIntervalleMs / 10);


        public Partie()
        {
            Console.WriteLine("Partie créée");

            listeClient = new List<Client>();
            listeProjectile = new List<Projectile>();

            chrono = new Stopwatch();

            graine = rnd.Next(1, 1000000);
            map = new Map(graine);

            map.GenTab();
            map.setTabObstacle(map.GenLisserTab(map.getTabObstacle(), 2));

            if (! GestionnaireItem.initialise) { GestionnaireItem.Initialiser(); }
        }

        ~Partie()
        {
            Console.WriteLine("Partie supprimée");
        }

        public void SupprimerPartie()
        {
            Console.WriteLine("test");
            verrouClient = true;
            foreach (Client c in listeClient)
            {
                c.partie = null;
            }
            verrouClient = false;

            listeProjectile.Clear();
            listePartie.Remove(this);
            suppr = true;
        }

        // Commence une partie, prévient les clients du début de la partie
        public void StartClient(Client c)
        {
            c.partie = this;
            c.EnvoyerMessage(string.Join(Client.sep2, ["p", graine.ToString(), c.id]));
        }


        // Actualisation de toute la map (positions, vies, ..)
        public void Actualiser()
        {
            verrouClient = true;

            // Items 
            string infosItems = "";
            List<string> infosItemsTab = new List<string>();
            for (int i = 0; i < GestionnaireItem.tabTypes.Length; i++)
            {
                SupprimerElementsMap(GestionnaireItem.dictItemInstance[GestionnaireItem.tabTypes[i]]);

                foreach (Item It in GestionnaireItem.dictItemInstance[GestionnaireItem.tabTypes[i]])
                {
                    infosItems += It.InfosItem();
                }

                if (infosItems.Length == 0) { infosItems = "*"; }
                infosItemsTab.Add(infosItems);
                infosItems = "";
            }
            infosItems = string.Join(Client.sep2, infosItemsTab);

            GestionnaireItem.Actualiser(map);


            // Actualisation des projectiles 
            // Pour optimiser, l'actualisation des objets se fait dans la même boucle que celle qui sert à récupérer les infos à envoyer aux client pour les actualiser
            SupprimerElementsMap(listeProjectile);
            string infosProjectiles = "";
            foreach (Projectile p in new List<Projectile>(listeProjectile))
            {
                p.Actualiser();
                infosProjectiles += string.Join(Client.sep4, [p.id.ToString(), p.x.ToString(), p.y.ToString(), p.direction.ToString(), p.idJoueur.ToString()]) + Client.sep3;
            }
            if (infosProjectiles.Length == 0) { infosProjectiles = "*"; }


            // Envoi du message d'actualisation pour les clients : Construction du message + vérif si y'en a un qui a gagné
            string infosJoueurs = "";
            Joueur? gagnant = null;
            foreach (Client c in listeClient)
            {
                if (c.joueur != null)
                {
                    Joueur j = c.joueur;

                    if (j.vie <= 0) { c.joueur = null; }
                    else
                    {
                        if (j.chrono.Elapsed.TotalSeconds > tempsVictoire) { gagnant = j; break; }
                        j.Actualiser();

                        infosJoueurs += string.Join(Client.sep4, [j.id.ToString(), j.pseudo, j.tempsAfkMs.ToString(), j.x.ToString(), j.y.ToString(), j.vie.ToString(), j.couleur, j.nbProjectiles, c.connecte.ToString()]) + Client.sep3;
                    }
                }
            }
            if (infosJoueurs.Length == 0) { infosJoueurs = "*"; }


            if (gagnant == null)
            {
                // Envoi du message d'actualisation pour les clients
                foreach (Client c in listeClient)
                {
                    c.EnvoyerMessage(string.Join(Client.sep2, ["a", infosJoueurs, infosProjectiles, infosItems]));
                }
            }
            else
            {
                // Envoi du message de fin de partie
                foreach (Client c in listeClient)
                {
                    c.EnvoyerMessage(string.Join(Client.sep2, ["g", gagnant.pseudo]));
                }

                Console.WriteLine("Partie terminée (id gagnant : " + gagnant.id.ToString() + " )");

                SupprimerPartie();
            }


            // Supprime la partie si elle est inutilisé
            if (!chrono.IsRunning)
            {
                if (listeClient.Count == 0) { chrono.Restart(); }
            }
            else
            {
                if (listeClient.Count != 0) { chrono.Stop(); }
                if (chrono.Elapsed.TotalSeconds > tempsSupprPartie)
                {
                    SupprimerPartie();
                }
            }

            verrouClient = false;
        }
    }
}
