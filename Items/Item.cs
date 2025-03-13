using Serveur_AFKSimulator.ObjectsMap;

namespace Serveur_AFKSimulator.Items
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class Item : ElementMap
    {
        // ------------------------------ Attributs STATIQUES ------------------------------


        public static bool[,] tabPosItem = new bool[Map.tailleMap, Map.tailleMap];


        // ------------------------------ M�thodes ------------------------------


        public Item(int id, int x, int y, Map map, int largeur = 30, int hauteur = 30) : base(id, Map.RowToX(x), Map.RowToY(y), map)
        {
            tabPosItem[x, y] = true;

            this.largeur = largeur;
            this.hauteur = hauteur;

            // Pour aligner l'item au centre de la cellule
            this.x += (Map.tailleCellMap - largeur) / 2;
            this.y += (Map.tailleCellMap - hauteur) / 2;
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
