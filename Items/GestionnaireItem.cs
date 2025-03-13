

using Serveur_AFKSimulator;
using System.Diagnostics;

namespace Serveur_AFKSimulator.Items
{

    /// <summary>
    ///     Gestionnaire d'items. 
    ///     
    ///     A LIRE! 
    ///     -> Pour chaque items créé, dupliquer une ligne dans la fonction CreerInstances selon l'item ajouté et ajouter une ligne au tableau statiques tabTypes et typeVariables. 
    ///     
    /// </summary>
    public class GestionnaireItem
    {

        // ------------------------------ Attributs STATIQUES ------------------------------

        /*
        // Les variables de tabVariables[i] sont les équivalents des attributs statiques du type tabTypes[i] !! Donc l'ordre est important
        public static Type[] tabTypes = [
            typeof(OrbeCouleur),
            typeof(Recharge)
        ];

        public static DataItems[] tabVariables = [
            new DataItems(2000, 13, new Stopwatch()),
            new DataItems(2000, 10, new Stopwatch())
        ];
        */

        // ------------------------------ Attributs ------------------------------


        public struct DataItems // Liste de variables de classe pour chaque type d'item
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

        // Les variables de tabVariables[i] sont les équivalents des attributs statiques du type tabTypes[i] !! Donc l'ordre est important
        public Type[] tabTypes;
        public DataItems[] tabVariables;

        public Dictionary<Type, DataItems> dictItemData = new();
        public Dictionary<Type, List<Item>> dictItemInstance = new();


        // ------------------------------ Méthodes STATIQUES ------------------------------


        public static (int, int) CelluleLibre(bool[,] tab1, bool[,] tab2)
        {

            int x;
            int y;
            Random rnd = new Random();
            do
            {
                x = rnd.Next(0, Map.tailleMap - 1);
                y = rnd.Next(0, Map.tailleMap - 1);

            }
            while (tab1[x, y] || tab2[x, y]);

            return (x, y);
        }


        // ------------------------------ Méthodes ------------------------------


        public GestionnaireItem(Type[] tabTypes, DataItems[] tabVariables)
        {
            if (tabTypes.Length != tabVariables.Length) { throw new Exception("Les tableaux tabTypes et tabData doivent être de même taille"); }

            this.tabTypes = tabTypes;
            this.tabVariables = tabVariables;

            for (int i = 0; i < tabTypes.Length; i++)
            {
                dictItemData[tabTypes[i]] = tabVariables[i];
                dictItemInstance[tabTypes[i]] = new List<Item>();
                tabVariables[i].chrono.Start();
            }
        }
        

        public Item CreerInstances(Type t, Map map, List<Item> liste)
        {
            int x;
            int y;
            (x, y) = CelluleLibre(map.tabBool, Item.tabPosItem);
            

            // ligne à dupliquer pour chaque type
            if (t == typeof(OrbeCouleur)) { return new OrbeCouleur(Identification.TrouverIdDispo(liste), x, y, map); }

            else { return new Recharge(Identification.TrouverIdDispo(liste), x, y, map); }
        }

        public void Actualiser(Map map)
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
