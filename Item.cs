


public abstract class Item : ElementMap
{
    public static int[,] tabItem = new int[Map.tailleMap, Map.tailleMap];


    public Item(int id, int x, int y) : base(id, x, y)
    {
        
    }

    // La vérification de collision se fait dans l'objet joueur, et il appelle cette méthode si il rencontre un item
    public abstract void RecupererItem(Joueur j);


}