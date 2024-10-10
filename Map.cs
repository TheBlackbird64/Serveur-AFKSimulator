

public class Map
{
    public long valRnd { get; set; }

    // Valeur de la carte 
    public const int coinMapH = 250;
    public const int coinMapG = 250;
    public const int tailleMap = 50;

    // Valeurs de génération
    public const double valInf = 0.8;
    public const int nbVoisins = 3;
    public const int nbDec = 3;
    public double[,] tabObstacle = new double[tailleMap, tailleMap];

    public Map(int graine)
    {
        valRnd = graine;
        GenRnd();
    }

    // Générateur congruentiel linéaire
    public long GenRnd()
    {
        valRnd = (long) ((16807 * valRnd + 1) % Math.Pow(2, 32));
        return valRnd;
    }

    public double GenRange(double xmin, double xmax, int nbDec = nbDec)
    {
        double puissance10 = (double) Math.Pow(10, nbDec);
        xmax *= puissance10;
        xmin *= puissance10;
        double diff = xmax - xmin;
        if (diff <= 0 || Math.Pow(2, 32) < diff) { return xmax; }

        return (double) ((GenRnd() % diff) + xmin) / puissance10;
    }

    public void GenTab()
    {
        for (int i = 0; tailleMap > i; i++)
        {
            for (int j = 0; tailleMap > j; j++)
            {
                tabObstacle[i, j] = GenRange(-1, 1, 2);
            }
        }
    }

    public List<double> TabVoisins(double[,] tab, int i, int j)
    {
        List<double> d = new List<double>();
        for (var i1 = -1; 1 >= i1; i1++)
        {
            for (var j1 = -1; 1 >= j1; j1++)
            {
                try
                {
                    d.Add(tab[i + i1, j + j1]);
    
                }
                catch { }
            }
        }

        return d;
    }

    public int ValVoisins(List<double> liste, double valInf, double valSup)
    {
        int res = 0;
        for (int i = 0; liste.Count > i; i++)
        {
            if (liste[i] >= valInf && liste[i] <= valSup) { res++; }
        }

        return res;
    }

    public double[,] GenLisserTab(double[,] tab, int n = 1)
    {
        double[,] tabLisse = new double[tailleMap, tailleMap];

        for (int i = 0; tailleMap > i; i++)
        {
            for (int j = 0; tailleMap > j; j++)
            {
                if (ValVoisins(TabVoisins(tab, i, j), valInf, 1) > nbVoisins)
                {
                    tabLisse[i, j] = GenRange(valInf, 1, 2);
                }
                else
                {
                    tabLisse[i, j] = GenRange(-1, valInf-Math.Pow(10, -nbDec), 3);
                }
            }
        }

        if (n > 1) { return GenLisserTab(tabLisse, n-1); }
        else { return tabLisse; }
    }

    public bool[,] TabBool()
    {
        bool[,] tabB = new bool[tailleMap, tailleMap];

        for (int i = 0; tailleMap > i; i++)
        {
            for (int j = 0; tailleMap > j; j++)
            {
                tabB[i, j] = tabObstacle[i, j] >= valInf;
            }
        }
        return tabB;
    }
}