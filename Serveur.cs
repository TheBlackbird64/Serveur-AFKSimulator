using System;
using System.Net.Sockets;
using System.Net;


public class Serveur
{
    private Socket listener;
    public List<Joueur> joueurListe { get; set; }

    public Serveur(int port, int maxConnexions)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1"); //ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

        listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //IPV6 : AddressFamily.InterNetworkV6
        listener.Bind(localEndPoint);
        listener.Listen(maxConnexions);

        joueurListe = new List<Joueur>();
    }

    public async Task Start()
    {
        Console.WriteLine(" -------------- Serveur démarré -------------- \n\n");

        while (true)
        {
            // Trouve un identifiant qui n'est pas déjà utilisé par un client
            int idClient = 0;
            bool existe = false;
            while (!existe)
            {
                idClient++;
                existe = true;
                foreach (Joueur c in joueurListe)
                {
                    if (c.id == idClient)
                    {
                        existe = false;
                    }
                }
            }

            Joueur cli = new Client(await listener.AcceptAsync(), idClient, this);
            joueurListe.Add(cli);

            cli.RecMessagesAsync();
            Partie.ActualiserParties();
        }
    }
}
