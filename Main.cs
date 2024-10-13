using System;
using System.Diagnostics;


public class Principale
{
	public static void Main()
	{
        /*
        Random rnd = new Random();
        Map m = new Map(50505);// rnd.Next(1, 1000000));
		m.GenTab();
        m.tabObstacle = m.GenLisserTab(m.tabObstacle);
        for (int i = 0; Map.tailleMap > i; i++)
        {
            for (int j = 0; Map.tailleMap > j; j++)
            {
                Console.Write(m.tabObstacle[i, j]);
                Console.Write("  ");
            }
        }*/

        Serveur s = new Serveur (10000, 100);
		Task t = s.Start();
		t.Wait();
		//*/
	}
}
