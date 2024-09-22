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
        
        
        Partie.ActualiserPartiesAsync();

        while (true)
        {

            Joueur cli = new Joueur(await listener.AcceptAsync(), ElementMap.TrouverIdDispo(new List<ElementMap> (joueurListe)), this);
            joueurListe.Add(cli);

            cli.RecMessagesAsync();
        }
    }
}
