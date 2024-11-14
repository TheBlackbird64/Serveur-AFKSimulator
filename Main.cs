using System.Text.Json;

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
