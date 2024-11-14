namespace Serveur_AFKSimulator.ObjectsMap
{
    public abstract class ElementMap : Identification
    {
        public int x { get; set; }
        public int y { get; set; }
        public int largeur { get; set; }
        public int hauteur { get; set; }
        public bool suppr { get; set; }
        public Map map { get; set; }


        // Tout les �l�ments qui sont sur la carte h�ritent de cette classe. Elle contient principalement diff�rentes fonctions de collisions.
        public ElementMap(int id, int x, int y, Map map)
        {
            // Toutes les coordonn�es partent d'en haut � droite du sprite de l'objet
            this.id = id;
            this.x = x;
            this.y = y;
            this.map = map;
            suppr = false;
        }


        public bool Collision(ElementMap elem1, ElementMap elem2)
        {
            return elem1.x + elem1.largeur > elem2.x && elem1.x < elem2.x + elem2.largeur && elem1.y + elem1.hauteur > elem2.y && elem1.y < elem2.y + elem2.hauteur;
        }

        public bool Collision(ElementMap elem1, bool[,] tabMap)
        {
            int x1;
            int y1;
            int x2;
            int y2;
            bool collision = false;

            for (int i = 0; 1 >= i; i++)
            {
                for (int j = 0; 1 >= j; j++)
                {
                    x1 = elem1.x + elem1.largeur * i;
                    y1 = elem1.y + elem1.hauteur * j;

                    x2 = Map.XToRow(x1);
                    y2 = Map.YToRow(y1);

                    if (i == 1 && x1 % Map.tailleCellMap == 0) { x2--; }
                    if (j == 1 && y1 % Map.tailleCellMap == 0) { y2--; }

                    if (x2 >= tabMap.GetLength(0) || y2 >= tabMap.GetLength(0) || x2 < 0 || y2 < 0)
                    {
                        collision = true;
                        break;
                    }
                    else
                    {
                        if (tabMap[x2, y2]) { collision = true; break; }
                    }
                }
                if (collision) { break; }
            }

            return collision;

        }

        // Fonction appel� � chaque �tape du jeu pour actualiser les �l�ments (collisions, d�placements, ...)
        public abstract void Actualiser();
    }

}
