﻿
using System.Net.Sockets;
using System.Net;
using Serveur_AFKSimulator.Items;

namespace Serveur_AFKSimulator
{
    public static class Serveur
    {
        // VARIABLES DE CONFIGURATION DU SERVEUR
        public static int maxConnexions;
        public static int port;

        // VARIABLES POUR LE SOCKET (on touche pas, ça a l'air de marcher)
        private static IPAddress? ipAddr; 
        private static IPEndPoint? localEndPoint;
        private static Socket? listener; 

        // VARIABLES AUTRES
        public static List<Client> clientListe { get; set; } = new List<Client>();


        public static async Task Start()
        {
            ipAddr = IPAddress.Parse("127.0.0.1"); //IPv6Any; //
            localEndPoint = new IPEndPoint(ipAddr, port);
            listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //AddressFamily.InterNetworkV6

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
