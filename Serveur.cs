
using System.Net.Sockets;
using System.Net;
using Serveur_AFKSimulator.Items;

namespace Serveur_AFKSimulator
{
    public static class Serveur
    {
        // VARIABLES DE CONFIGURATION DU SERVEUR
        private static int maxConnexions = 100;
        private static int port = 10000;
        private static string adr = "127.0.0.1";

        // VARIABLES POUR LE SOCKET (on touche pas, ça a l'air de marcher)
        private static IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddr = IPAddress.Parse(adr); //ipHost.AddressList[0];
        private static IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
        private static Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //IPV6 : AddressFamily.InterNetworkV6;

        // VARIABLES AUTRES
        public static List<Joueur> joueurListe { get; set; } = new List<Joueur>();


        public static async Task Start()
        {
            listener.Bind(localEndPoint);
            listener.Listen(maxConnexions);

            Console.WriteLine(" -------------- Serveur démarré -------------- \n\n");

            Partie.ActualiserPartiesAsync();

            while (true)
            {

                Joueur cli = new Joueur(await listener.AcceptAsync(), Identification.TrouverIdDispo(joueurListe));
                joueurListe.Add(cli);

                cli.RecMessagesAsync();
            }
        }
    }
}
