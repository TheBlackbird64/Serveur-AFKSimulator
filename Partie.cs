

public class Partie
{
    public static List<Partie> parties = new List<Partie>();
    public static List<Client> fileAttente = new List<Client>();
    public static int nbJoueursMin = 5;
    public static int nbJoueursMax = 10;

    public List<Client> listeJoueurs { get; set; }
    public int nbJoueurs { get { return listeJoueurs.Count; } }
    public int graine { get; set; }
    public bool started { get; set; }


    public static void CreerParties()
    {
        if (fileAttente.Count >= nbJoueursMin)
        {
            Partie p = new Partie();
            parties.Add(p);
        }


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
    }


    public Partie()
    {
        listeJoueurs = new List<Client>();
        Random rnd = new Random();
        graine = rnd.Next(1000, 9999);
    }

    public void Start()
    {
        started = true;
        foreach (Client client in listeJoueurs)
        {
            client.partie = this;
            client.EnvoyerMessage(string.Join(Client.sep2, ["p", graine.ToString(), client.id]));
        }
    }
}