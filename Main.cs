using System.Text.Json;
using System.Net;

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
