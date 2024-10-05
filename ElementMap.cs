

public abstract class ElementMap
{
    public int id { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int largeur { get; set; }
    public int hauteur { get; set; }
    public bool suppr { get; set; }

    // Trouve un id pas utilisé parmi une liste d'éléments ElementMap
    public static int TrouverIdDispo(List<ElementMap> listeElem)
    {
        int idElem = 0;
        bool dejaPresent = true;
        while (dejaPresent)
        {
            idElem++;
            dejaPresent = false;
            foreach (ElementMap e in listeElem)
            {
                if (e.id == idElem)
                {
                    dejaPresent = true;
                }
            }
        }

        return idElem;
    }


    public ElementMap(int id, int x, int y)
    {
        // Toutes les coordonnées partent d'en haut à droite
        this.id = id;
        this.x = x;
        this.y = y;
        suppr = false;
    }

    public bool Collision(ElementMap elem1, ElementMap elem2)
    {
        return ((elem1.x + elem1.largeur > elem2.x) && (elem1.x < elem2.x + elem2.largeur) && (elem1.y + elem1.hauteur > elem2.y) && (elem1.y < elem2.y + elem2.hauteur));
    }

    // Fonction appelé à chaque étape du jeu pour actualiser les éléments (collisions, déplacements, ...)
    public abstract void Actualiser();
}