

public class ElementMap
{
    public int id { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int largeur { get; set; }
    public int hauteur { get; set; }

    public ElementMap(int id, int x, int y)
    {
        // Toutes les coordonnées partent d'en haut à droite
        this.id = id;
        this.x = x;
        this.y = y;
    }

    public bool Collision(ElementMap elem1, ElementMap elem2)
    {
        return ((elem1.x + elem1.largeur > elem2.x) && (elem1.x < elem2.x + elem2.largeur) && (elem1.y + elem1.hauteur > elem2.y) && (elem1.y < elem2.y + elem2.hauteur));
    }
}