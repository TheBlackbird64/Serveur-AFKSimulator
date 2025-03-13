
namespace Serveur_AFKSimulator
{
    public class Map
    {
        // ------------------------------ Attributs ------------------------------


        public long valRnd { get; set; }

        // constantes de la carte 
        public const int coinMapH = 250;
        public const int coinMapG = 250;
        public const int tailleMap = 50;
        public const int tailleCellMap = 50;

        // constantes de génération
        public const double valInf = 0.6;
        public const int nbVoisins = 3;
        public const int nbDec = 3;

        // Tableaux 
        private double[,] tabObstacle = new double[tailleMap, tailleMap];
        public bool[,] tabBool = new bool[tailleMap, tailleMap];


        // ------------------------------ Méthodes STATIQUES ------------------------------


        /// <summary> Convertit une coordonnée x en indice de ligne de tableau de taille tailleMap. </summary>
        public static int XToRow(int x) => Convert.ToInt32(Math.Floor(((double) (x - coinMapG)) / ((double) tailleCellMap)));

        /// <summary> Convertit une coordonnée y en indice de ligne de tableau de taille tailleMap. </summary>
        public static int YToRow(int y) => Convert.ToInt32(Math.Floor(((double) (y - coinMapG)) / ((double) tailleCellMap)));

        /// <summary> Convertit un indice de ligne de tableau de taille tailleMap en une coordonnée x. </summary>
        public static int RowToX(int x) => tailleCellMap * x + coinMapG;

        /// <summary> Convertit un indice de ligne de tableau de taille tailleMap en une coordonnée y. </summary>
        public static int RowToY(int y) => tailleCellMap * y + coinMapH;


        // ------------------------------ Méthodes ------------------------------


        public Map(int graine)
        {
            valRnd = graine;
        }

        public void setTabObstacle(double[,] tab)
        {
            tabObstacle = tab;
            for (int i = 0; tailleMap > i; i++)
            {
                for (int j = 0; tailleMap > j; j++)
                {
                    tabBool[i, j] = tabObstacle[i, j] >= valInf;
                }
            }
        }

        public void setTabObstacle(int i, int j, double val)
        {
            tabObstacle[i, j] = val;
            tabBool[i, j] = val >= valInf;
        }

        public double[,] getTabObstacle() => tabObstacle;


        // Fonctions de generation de la map

        /// <summary> Générateur congruentiel linéaire </summary>
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
                    setTabObstacle(i, j, GenRange(-1, 1));
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
    }
}
