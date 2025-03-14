﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Serveur_AFKSimulator.ObjectsMap;
using Serveur_AFKSimulator.Items;

namespace Serveur_AFKSimulator
{
    public class Client : Identification
    {

        // ------------------------------ Attributs STATIQUES ------------------------------

        public static int tempsSupprClient;


        // ------------------------------ Attributs ------------------------------


        // VARIABLES RESEAU
        private Socket sock;
        private bool sockAFermer = true;
        private string msgpart = "";
        public bool connecte { get; set; } = true;
        

        // SEPARATEURS MESSAGES RESEAU
        public const string sep1 = "|";
        public const string sep2 = ",";
        public const string sep3 = ";";
        public const string sep4 = "!";

        // VARIABLES FONCTIONNEMENT
        private string pseudo;
        public Partie? partie {  get; set; }
        public Joueur? joueur { get; set; }
        public Stopwatch chrono { get; set; }


        // ------------------------------ Méthodes ------------------------------


        public Client(Socket socket, int _id)
        {
            sock = socket;
            chrono = new Stopwatch();
            id = _id;
            pseudo = "";

            Log(" + Client connecté");
        }

        ~Client() {
            Log("--- Client supprimé");
        }


        public async void RecMessagesAsync()
        {
            // Si le client est déconnecté on attend de voir si il se reconnecte avant de suppr l'objet
            do
            {
                // Recevoir les messages
                while (connecte)
                {
                    if (chrono.IsRunning) { chrono.Reset(); }

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
                                Log(" > Client deconnecte");
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
                            if ((i == msgTab.Count() - 1) && (msgTab[i] != ""))
                            {
                                msgpart = msgTab[i];
                            }
                            else
                            {
                                TraiterMessagesAsync(msgTab[i].Split(sep2));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log(" > Client deconnecte suite a une erreur: " + e.Message);
                        connecte = false;
                    }
                }


                if (! chrono.IsRunning) { chrono.Start(); }

                await Task.Delay(10);
            }
            while (chrono.ElapsedMilliseconds < tempsSupprClient);

            await SupprimerClientAsync();
        }


        public async void TraiterMessagesAsync(string[] msg)
        {
            if (msg[0] == "c") //  forme du message : ["c"]  Connexion récupérée (un client s'est déco/reco, il faut le remettre dans l'objet Client ds lequel il est, on retrouve cet objet grace à l'id
            {
                bool err = false;
                int _id = -1;
                try
                {
                    _id = int.Parse(msg[1]);
                }
                catch { err = true; }

                if (!err)
                {
                    bool trouve = false;
                    foreach (Client c in Serveur.clientListe)
                    {
                        if (!c.connecte && c.id == _id)
                        {
                            c.sock = sock;
                            sockAFermer = false;

                            c.connecte = true;
                            trouve = true;
                            break;
                        }
                    }

                    if (trouve)
                    {
                        EnvoyerMessage("c"); // Si trouve continue a marcher comme si de rien était
                        await SupprimerClientAsync();
                    }
                    else
                    {
                        EnvoyerMessage("rst"); // Reset le client si l'objet n'est pas trouvé (repart sur ecran accueil)
                    }
                }
            }
            else if (msg[0] == "j") // forme du message : ["j", pseudo]    Jouer (mis en attente pour une partie)
            {

                pseudo = msg[1];
                if ((!Partie.fileAttente.Contains(this)) && partie == null)
                {
                    Partie.fileAttente.Add(this);
                    // Donner les infos de config.json au client
                    EnvoyerMessage(string.Join(Client.sep2, ["info", Partie.nbJoueursMin, Partie.actualiserIntervalle, Joueur.vieMax, OrbeCouleur.valAjoutCouleur]));
                }
            }
            else if (msg[0] == "r") //  forme du message : ["r"]  Rejouer (aller dans la partie)
            {
                if (partie != null)
                {
                    joueur = new Joueur(partie, id, pseudo);
                }
            }
            else
            {
                if (joueur != null)
                {
                    joueur.TraiterMessages(msg);
                }
            }
        }


        public void EnvoyerMessage(string msg)
        {
            try
            {
                sock.Send(Encoding.UTF8.GetBytes(msg + sep1));
            }
            catch
            {
                // Erreur lors de l'envoi d'un message au client, pas de déconnexion du client car cela risque de modifier une liste pendant son itération dans une boucle
            }

        }


        public async Task SupprimerClientAsync()
        {
            // Attendre que la partie autorise la suppression
            if (partie != null) 
            { 
                while (partie.verrouClient)
                {
                    await Task.Delay(20);
                }
            }

            // Vider les listes pouvant contenir le client
            Serveur.clientListe.Remove(this);
            if (Partie.fileAttente.Contains(this)) { Partie.fileAttente.Remove(this); }
            
            if (partie != null) {
                if (partie.listeClient.Contains(this)) { partie.listeClient.Remove(this); } 
            }

            if (sockAFermer) { sock.Close(); }
        }


        public void Log(string msg)
        {
            Serveur.Log("  " + msg + " (id:" + id.ToString() + ")");
        }
    }
}
