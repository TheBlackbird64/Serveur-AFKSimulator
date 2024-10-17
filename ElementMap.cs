
namespace AFKSimulator
{
    public abstract class ElementMap
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int largeur { get; set; }
        public int hauteur { get; set; }
        public bool suppr { get; set; }

        // Trouve un id pas utilisé parmi une liste d'éléments ElementMap (id != 0)
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
            return (elem1.x + elem1.largeur > elem2.x) && (elem1.x < elem2.x + elem2.largeur) && (elem1.y + elem1.hauteur > elem2.y) && (elem1.y < elem2.y + elem2.hauteur);
        }

        public (int, int) CollisionGetPos(ElementMap elem1, bool[,] tabMap)
        {
            int x2 = -1;
            int y2 = -1;
            bool collision = false;

            for (int i = 0; 1 >= i; i++)
            {
                for (int j = 0; 1 >= j; j++)
                {
                    x2 = Map.XToRow(elem1.x + elem1.largeur * i);
                    y2 = Map.YToRow(elem1.y + elem1.hauteur * j);

                    if (tabMap[x2, y2]) { collision = true; break; }
                }
                if (collision) { break; }
            }

            if (collision) { return (x2, y2); }
            else { return (-1, -1); }

        }

        public bool Collision(ElementMap elem1, bool[,] tabMap)
        {
            int x, y;
            (x, y) = CollisionGetPos(elem1, tabMap);
            return (x != -1 && y != -1);
        }

        // Fonction appelé à chaque étape du jeu pour actualiser les éléments (collisions, déplacements, ...)
        public abstract void Actualiser();
    }

}
