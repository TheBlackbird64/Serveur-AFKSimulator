
namespace Serveur_AFKSimulator
{
    public class Map
    {
        public long valRnd { get; set; }

        // Valeur de la carte 
        public const int coinMapH = 250;
        public const int coinMapG = 250;
        public const int tailleMap = 50;
        public const int tailleCellMap = 50;

        // Valeurs de génération
        public const double valInf = 0.6;
        public const int nbVoisins = 3;
        public const int nbDec = 3;
        public double[,] tabObstacle = new double[tailleMap, tailleMap];

        // Convertit une coordonnée x en indice de ligne de tableau de taille tailleMap.
        public static int XToRow(int x) => (x - coinMapG) / tailleCellMap;
        public static int YToRow(int y) => (y - coinMapH) / tailleCellMap;
        

        public Map(int graine)
        {
            valRnd = graine;
        }

        // Générateur congruentiel linéaire
        public long GenRnd()
        {
            valRnd = (long)((16807 * valRnd + 1) % Math.Pow(2, 32));
            return valRnd;
        }

        public double GenRange(double xmin, double xmax, int nbDec = nbDec)
        {
            double puissance10 = (double)Math.Pow(10, nbDec);
            xmax *= puissance10;
            xmin *= puissance10;
            double diff = xmax - xmin;
            if (diff <= 0 || Math.Pow(2, 32) < diff) { return xmax; }

            return (double)((GenRnd() % diff) + xmin) / puissance10;
        }

        public void GenTab()
        {
            for (int i = 0; tailleMap > i; i++)
            {
                for (int j = 0; tailleMap > j; j++)
                {
                    tabObstacle[i, j] = GenRange(-1, 1);
                }
            }

        }

        public List<double> TabVoisins(double[,] tab, int i, int j)
        {
            List<double> d = new List<double>();
            for (int i1 = -1; 1 >= i1; i1++)
            {
                for (int j1 = -1; 1 >= j1; j1++)
                {
                    if (i + i1 >= 0 && j + j1 >= 0 && i + i1 < tailleMap && j + j1 < tailleMap)
                    {
                        d.Add(tab[i + i1, j + j1]);
                    }
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
                        tabLisse[i, j] = GenRange(valInf, 1);
                    }
                    else
                    {
                        tabLisse[i, j] = GenRange(-1, valInf - Math.Pow(10, -nbDec));
                    }
                }
            }

            if (n > 1) { return GenLisserTab(tabLisse, n - 1); }
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
}
