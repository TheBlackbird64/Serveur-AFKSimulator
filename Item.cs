


public abstract class Item : ElementMap
{
    public static Item[,] tabItem = new Item[Map.tailleMap, Map.tailleMap];
    public static bool[,] tabItemBool = new bool[Map.tailleMap, Map.tailleMap];


    public Item(int id, int x, int y) : base(id, x, y)
    {
        tabItem[Map.XToRow(x), Map.YToRow(y)] = this;
    }

    // La vérification de collision se fait dans l'objet joueur, et il appelle cette méthode si il rencontre un item
    public void RecupererItem(Joueur j)
    {
        suppr = true;
        
    }


}