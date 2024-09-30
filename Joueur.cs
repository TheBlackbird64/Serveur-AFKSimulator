using System;
using System.Net.Sockets;
using System.Numerics;
using System.Text;



public class Joueur : ElementMap
{

    private Socket sock;
    private Serveur serveur;
    public Partie? partie;
    private string msgpart = "";

    public string pseudo { get; set; }
    public int vie { get; set; }
    public string couleur { get; set; }
    public int tempsAfkMs { get; set; }
    public bool connecte { get; set; } = true;
    public bool suppr { get; set; } = false;
    
    public const string sep1 = "|";
    public const string sep2 = ",";
    public const string sep3 = ";";
    public const string sep4 = "!";

    public Joueur(Socket socket, int _id, Serveur serv) : base(_id, 0, 0)
    {
        
        sock = socket;
        serveur = serv;

        pseudo = "";
        vie = 0;
        couleur = "000000";
        tempsAfkMs = 0;

        Log("Client connecté");
    }

    public void ResetVars()
    {
        x = 0;
        y = 0;
        vie = 100;
        couleur = "000000";
        tempsAfkMs = 0;
    }

    public async void RecMessagesAsync()
    {
        while (connecte)
        {
            byte[] buffer = new byte[1024];
            string message = "";

            try
            {
                int bytesRead = 1024;
                while (bytesRead == 1024)
                {
                    bytesRead = 0;
                    bytesRead = await sock.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead > 0)
                    {
                        message += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                    else
                    {
                        Log("Client deconnecte");
                        connecte = false;
                    }
                }

                // au cas ou le dernier message n'arrive pas complet, on garde ce qui est arrivé en mémoire pour le reconstituer
                if (message != "")
                {
                    message = msgpart + message; 
                    msgpart = "";
                }

                string[] msgTab = message.Split(sep1);
                for (int i = 0; msgTab.Count() > i; i++)
                {
                    if ((i == msgTab.Count()-1) && (msgTab[i] != ""))
                    {
                        msgpart = msgTab[i];
                    }
                    else
                    {
                        TraiterMessages(msgTab[i].Split(sep2));
                    }
                }
            }
            catch (Exception e)
            {
                Log("Client deconnecte suite a une erreur: " + e.Message);
                connecte = false;
            }
        }

        SupprimerClient();
    }

    public void SupprimerClient()
    {
        // Vider les listes pouvant contenir le client
        suppr = true;
        serveur.joueurListe.Remove(this);
        
        sock.Close();
    }

    public void TraiterMessages(String[] msg)
    {
        if (msg[0] == "j") // Jouer (mis en attente pour une partie)
        {
            pseudo = msg[1];
            if ((! Partie.fileAttente.Contains(this)) && partie == null)
            {
                Partie.fileAttente.Add(this);
            }
        }
        else if (msg[0] == "a") // Actualiser position et éventuellement tir
        {
            if (partie != null && partie.started)
            {
                x = int.Parse(msg[1]);
                y = int.Parse(msg[2]);

                // Si msg[2] n'est pas égal à -1, c'est que cette valeur est la direction du projectile tiré
                // On crée le projectile aux coordonnés du joueur avec la direction indiquée dans le message
                if (msg[3] != "-1")
                {
                    partie.listeProjectile.Add(new Projectile(ElementMap.TrouverIdDispo(new List<ElementMap>(partie.listeProjectile)), x, y, id, int.Parse(msg[3]), partie));
                }
            }
            
        }
        else if (msg[0] == "r") // Rejouer (aller dans la partie)
        {
            ResetVars();
        }
    }

    public void EnvoyerMessage(String msg)
    {
        try
        {
            sock.Send(Encoding.UTF8.GetBytes(msg + sep1));
        } catch 
        {
            // Erreur lors de l'envoi d'un message au client, pas de déconnexion du client car cela risque de modifier une liste pendant son itération dans une boucle
        }
        
    }

    public override void Actualiser()
    {
        // Les actions du joueur sont controlés par la partie réseau, fonction RecMessagesAsync()
        // Mettre ici éventuellement un anticheat (vérif de positions pour vois si le joueur passe dans un mur ou si sa vitesse est trop importante

        // Calcul du temps AFK

    }

    public void Log(String msg)
    {
        Console.WriteLine("  " + msg + " (id:" + id.ToString() + ")");
    }
}