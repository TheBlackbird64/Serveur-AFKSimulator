
namespace Serveur_AFKSimulator
{

    public class Principale
    {
        public static void Main()
        {
            Task t = Serveur.Start();
            t.Wait();

        }
    }
}
