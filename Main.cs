using System;


public class Principale
{
	public static void Main()
	{
        Serveur s = new Serveur (10000, 100);
		Task t = s.Start();
		t.Wait();
	}
}
