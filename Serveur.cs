
using System.Net.Sockets;
using System.Net;
using Serveur_AFKSimulator.Items;

namespace Serveur_AFKSimulator
{
    public static class Serveur
    {
        // VARIABLES DE CONFIGURATION DU SERVEUR
        private static int maxConnexions = 100;
        private static int port = 8300;

        // VARIABLES POUR LE SOCKET (on touche pas, ça a l'air de marcher)
        private static IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        private static IPAddress ipAddr = IPAddress.IPv6Any; //Parse("127.0.0.1");
        private static IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
        private static Socket listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp); //IPV6 : AddressFamily.InterNetworkV6;

        // VARIABLES AUTRES
        public static List<Client> clientListe { get; set; } = new List<Client>();


        public static async Task Start()
        {
            listener.Bind(localEndPoint);
            listener.Listen(maxConnexions);

            Console.WriteLine(" -------------- Serveur démarré -------------- \n\n");

            Partie.ActualiserPartiesAsync();

            while (true)
            {

                Client cli = new Client(await listener.AcceptAsync(), Identification.TrouverIdDispo(clientListe));
                clientListe.Add(cli);

                cli.RecMessagesAsync();
            }
        }
    }
}
