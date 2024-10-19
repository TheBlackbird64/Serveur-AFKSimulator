

using AFKSimulator;
using System.Diagnostics;

namespace Serveur_AFKSimulator.Items
{
    public static class GestionnaireItem
    {
        // Variables
        public static bool initialise = false;

        public struct DataItems
        {
            public int tpsApparitionMs;
            public int maxItem;
            public Stopwatch chrono;

            public DataItems(int tpsApparitionMs, int maxItem, Stopwatch chrono)
            {
                this.tpsApparitionMs = tpsApparitionMs;
                this.maxItem = maxItem;
                this.chrono = chrono;
            }
        }

        public static Dictionary<Type, DataItems> dictItemData = new();
        public static Dictionary<Type, List<Item>> dictItemInstance = new();


        // Les variables de tabVariables[i] sont celle du type tabTypes[i] !! donc attention à l'ordre ds ces tableaux
        // Ces 2 arrays sont à modifier pour chaque classe fille de Item
        // Modifier aussi la fonction CreerInstances
        public static Type[] tabTypes = [
            typeof(OrbeCouleur)
            ];

        public static DataItems[] tabVariables = [
            new DataItems(2000, 13, new Stopwatch())
            ];



        public static (int, int) CelluleLibre(bool[,] tab1, bool[,] tab2)
        {
            int x = 0;
            int y = 0;
            Random rnd = new Random();
            do
            {
                x = rnd.Next(0, Map.tailleMap-1);
                y = rnd.Next(0, Map.tailleMap-1);
            }
            while (tab1[x, y] || tab2[x, y]);

            return (x * Map.tailleCellMap + Map.coinMapG, y * Map.tailleCellMap + Map.coinMapH);
        }

        public static Item CreerInstances(Type t, Map map, List<Item> liste)
        {
            int x = 0;
            int y = 0;
            (x, y) = CelluleLibre(map.TabBool(), Item.tabPosItem);

            if (t == typeof(OrbeCouleur)) { return new OrbeCouleur(ElementMap.TrouverIdDispo(new List<ElementMap> (liste)), x, y); }
            
            else { return new OrbeCouleur(ElementMap.TrouverIdDispo(new List<ElementMap>(liste)), x, y); }
        }

        public static void Initialiser()
        {
            initialise = true;
            for (int i = 0; i < tabTypes.Length; i++)
            {

                dictItemData[tabTypes[i]] = tabVariables[i];
                dictItemInstance[tabTypes[i]] = new List<Item>();
                tabVariables[i].chrono.Start();
            }
        }

        public static void Actualiser(Map map)
        {

            for (int i = 0; i < tabTypes.Length; i++)
            {
                if (dictItemData[tabTypes[i]].chrono.ElapsedMilliseconds > dictItemData[tabTypes[i]].tpsApparitionMs && dictItemInstance[tabTypes[i]].Count < dictItemData[tabTypes[i]].maxItem)
                {
                    dictItemData[tabTypes[i]].chrono.Restart();
                    dictItemInstance[tabTypes[i]].Add(CreerInstances(tabTypes[i], map, dictItemInstance[tabTypes[i]]));
                }
            }
        }
    }
}
