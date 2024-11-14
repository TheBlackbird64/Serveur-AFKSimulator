using Serveur_AFKSimulator.ObjectsMap;

namespace Serveur_AFKSimulator.Items
{
    // Pour chaque classe fille de Item, configurer parametres dans le gestionnaire d'item (arrays)
    public abstract class Item : ElementMap
    {
        public static bool[,] tabPosItem = new bool[Map.tailleMap, Map.tailleMap];

        public Item(int id, int x, int y, Map map) : base(id, x * Map.tailleCellMap + Map.coinMapG, y * Map.tailleCellMap + Map.coinMapH, map)
        {
            tabPosItem[x, y] = true;
        }

        ~Item()
        {
            tabPosItem[Map.XToRow(x), Map.YToRow(y)] = false;
        }

        // La vérification de collision se fait dans l'objet joueur, et il appelle cette méthode si il rencontre un item
        public abstract void RecupererItem(Joueur j);

        public abstract string InfosItem();
    }
}
