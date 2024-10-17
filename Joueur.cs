
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace AFKSimulator
{
    public class Joueur : ElementMap
    {

        private Socket sock;
        private Serveur serveur;
        public Partie? partie;
        private string msgpart = "";

        public string pseudo { get; set; }
        public int vie { get; set; }
        public string colRouge { get; set; }
        public string colVert { get; set; }
        public string colBleu { get; set; }
        public string couleur { get { return colRouge + colVert + colBleu; } }
        public long tempsAfkMs { get { return chrono.ElapsedMilliseconds; } }
        public bool connecte { get; set; } = true;
        public Stopwatch chrono { get; set; }

        public const string sep1 = "|";
        public const string sep2 = ",";
        public const string sep3 = ";";
        public const string sep4 = "!";

        public Joueur(Socket socket, int _id, Serveur serv) : base(_id, 0, 0)
        {

            sock = socket;
            serveur = serv;
            chrono = new Stopwatch();

            pseudo = "";
            vie = 0;
            colRouge = "00";
            colVert = "00";
            colBleu = "00";

            largeur = 50;
            hauteur = 50;

            Log("Client connect�");
        }

        public void ResetVars()
        {
            x = 0;
            y = 0;
            vie = 100;

            colRouge = "00";
            colVert = "00";
            colBleu = "00";

            if (chrono.IsRunning) { chrono.Restart(); }
            else { chrono.Start(); }
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

                    // au cas ou le dernier message n'arrive pas complet, on garde ce qui est arriv� en m�moire pour le reconstituer
                    if (message != "")
                    {
                        message = msgpart + message;
                        msgpart = "";
                    }

                    string[] msgTab = message.Split(sep1);
                    for (int i = 0; msgTab.Count() > i; i++)
                    {
                        if ((i == msgTab.Count() - 1) && (msgTab[i] != ""))
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
                if (chrono.IsRunning) { chrono.Reset(); }
                pseudo = msg[1];
                if ((!Partie.fileAttente.Contains(this)) && partie == null)
                {
                    Partie.fileAttente.Add(this);
                }
            }
            else if (msg[0] == "a") // Actualiser position et �ventuellement tir
            {
                if (partie != null && partie.started && vie > 0)
                {
                    bool err = false;
                    int dirProjectile = 0;
                    int x2 = x;
                    int y2 = y;
                    try
                    {
                        x2 = int.Parse(msg[1]);
                        y2 = int.Parse(msg[2]);
                        dirProjectile = int.Parse(msg[3]);
                    }
                    catch { err = true; }

                    if (!err)
                    {
                        // Si le joueur bouge son chrono est remis � 0
                        if (x != x2 || y != y2)
                        {
                            chrono.Restart();
                        }

                        x = x2;
                        y = y2;

                        // Si msg[2] n'est pas �gal � -1, c'est que cette valeur est la direction du projectile tir�
                        // On cr�e le projectile aux coordonn�s du joueur avec la direction indiqu�e dans le message
                        if (msg[3] != "-1")
                        {
                            partie.listeProjectile.Add(new Projectile(ElementMap.TrouverIdDispo(new List<ElementMap>(partie.listeProjectile)), x, y, id, dirProjectile, partie, partie.map));
                        }
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
            }
            catch
            {
                // Erreur lors de l'envoi d'un message au client, pas de d�connexion du client car cela risque de modifier une liste pendant son it�ration dans une boucle
            }

        }

        public override void Actualiser()
        {
            // Les actions du joueur sont control�s par la partie r�seau, fonction RecMessagesAsync()
            // Mettre ici �ventuellement un anticheat (v�rif de positions pour vois si le joueur passe dans un mur ou si sa vitesse est trop importante

            if (partie != null)
            {

            }

            if (vie <= 0)
            {
                chrono.Stop();
                x = 0;
                y = 0;
            }
        }

        public void Log(String msg)
        {
            Console.WriteLine("  " + msg + " (id:" + id.ToString() + ")");
        }
    }
}
