
using AFKSimulator;

namespace Serveur_AFKSimulator.Items
{
    public abstract class Item : ElementMap
    {

        public Item(int id, int x, int y) : base(id, x, y) { }

        // La v�rification de collision se fait dans l'objet joueur, et il appelle cette m�thode si il rencontre un item
        public abstract void RecupererItem(Joueur j);

    }
}
