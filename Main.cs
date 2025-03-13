using System.Text.Json;
using System.Net;
using Serveur_AFKSimulator.Items;
using Serveur_AFKSimulator.ObjectsMap;
using System.Runtime.InteropServices;

namespace Serveur_AFKSimulator
{

    public class Principale
    {
        public static void Main()
        {
            Configuration.LoadConfig();

            
            Task t = Serveur.Start();
            t.Wait();
            
        }
    }
}
