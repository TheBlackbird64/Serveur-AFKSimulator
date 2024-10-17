
using AFKSimulator;

namespace Serveur_AFKSimulator.Items
{
    // Pour chaque classe fille de Item, configurer parametres dans le gestionnaire d'item (arrays)
    public abstract class Item : ElementMap
    {
        public static bool[,] tabPosItem = new bool[Map.tailleMap, Map.tailleMap];

        public Item(int id, int x, int y) : base(id, x, y)
        {
            tabPosItem[Map.XToRow(x), Map.YToRow(y)] = true;
        }

        ~Item()
        {
            tabPosItem[Map.XToRow(x), Map.YToRow(y)] = false;
        }

        // La v�rification de collision se fait dans l'objet joueur, et il appelle cette m�thode si il rencontre un item
        public abstract void RecupererItem(Joueur j);

        public abstract string InfosItem();
    }
}