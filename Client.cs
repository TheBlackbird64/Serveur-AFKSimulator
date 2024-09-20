using System;
using System.Net.Sockets;
using System.Text;



public class Client
{

    private Socket sock;
    private Serveur serveur;
    public Partie? partie;
    private String msgpart = "";

    public int id { get; set; }
    public String pseudo { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int vie { get; set; }
    public String couleur { get; set; }
    public int tempsAfkMs;

    public const String sep1 = "|";
    public const String sep2 = ",";
    public const String sep3 = ";";
    public const String sep4 = "!";


    public Client(Socket socket, int _id, Serveur serv)
    {
        
        sock = socket;
        serveur = serv;

        id = _id;
        pseudo = "";
        x = 0;
        y = 0;
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
        bool connecte = true;

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

                String[] msgTab = message.Split(sep1);
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
        // Vider les listes contenant le client
        serveur.clientListe.Remove(this);
        if (partie.fileAttente.Contains(this)) { partie.fileAttente.Remove(this); }
        if (partie.listeJoueurs.Contains(this)) { partie.listeJoueurs.Remove(this); }

        sock.Close();
        Log(serveur.clientListe.Count().ToString());
    }

    public void TraiterMessages(String[] msg)
    {
        if (msg[0] == "j") // Jouer (mis en attente pour une partie)
        {
            pseudo = msg[1];
            Partie.fileAttente.Add(this);
        }
        else if (msg[0] == "a") // Actualiser position et éventuellement tir
        {
            x = int.Parse(msg[1]);
            y = int.Parse(msg[2]);

            if (msg[2] != "-1")
            {
                // lancer projectile
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
            // Erreur lors de l'envoi d'un message au client, déconnexion du client
            connecte = false;
            SupprimerClient();
        }
        
    }

    public void Log(String msg)
    {
        Console.WriteLine("  " + msg + " (id:" + id.ToString() + ")");
    }
}