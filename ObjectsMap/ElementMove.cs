using System.ComponentModel;


namespace Serveur_AFKSimulator.ObjectsMap
{
    /// <summary>
    ///     Tout les éléments qui peuvent bouger héritent de cette classe qui contient toutes les fonctions de mouvement.
    /// </summary>
    
    public abstract class ElementMove : ElementMap
    {
        // ------------------------------ Attributs ------------------------------


        public int vitesse { get; set; } // Vitesse de déplacement
        public int vitRecul { get; set; } // Vitesse de recul
        public int vitRalentiRecul { get; set; } // Ralentissement à chaque actualisation pour que l'objet ne recule pas à l'infini
        public int directionRecul { get; set; } // direction du recul
        public int xPrevious { get; set; } // Valeur de X au mouvement précédente (pr pouvoir annuler un déplacement)
        public int yPrevious { get; set; } // Pareil pr Y


        // ------------------------------ Méthodes ------------------------------


        // Les fonctions de déplacement déplacent l'instance, et si un obstacle est rencontré la fonction ObstacleTouche() est appelée

        public ElementMove(int id, int x, int y, Map map) : base(id, x, y, map)
        {


            vitRalentiRecul = (int)Partie.ValeurSync(1);
            vitRecul = 0;
            directionRecul = 0;
            xPrevious = x;
            yPrevious = y;
        }


        

        public void ResetPrevious()
        {
            xPrevious = x;
            yPrevious = y;
        }

        /// <summary> L'action à faire en cas de collision est à définir pour chaque classe fille </summary>
        public abstract void ObstacleTouche();


        public void Deplacer(double vit, int dir)
        {
            ResetPrevious();

            x += Convert.ToInt32(Math.Round(Math.Cos(dir * Math.PI / 180.0) * vit));
            y += Convert.ToInt32(Math.Round(Math.Sin(-dir * Math.PI / 180.0) * vit));

            if (Collision(this, map.tabBool)) { ObstacleTouche(); }
        }

        public void DeplacerX(int depx)
        {
            if (depx != 0)
            {
                ResetPrevious();
                x += depx;
                if (Collision(this, map.tabBool))
                {
                    x = xPrevious;
                    DeplacerX(depx - 1 * (depx / Math.Abs(depx)));
                }
            }
        }

        public void DeplacerY(int depy)
        {
            if (depy != 0)
            {
                ResetPrevious();
                y += depy;
                if (Collision(this, map.tabBool))
                {
                    y = yPrevious;
                    DeplacerY(depy - 1 * (depy / Math.Abs(depy)));
                }
            }
        }


        /// <summary> Initialise un mouvement de recul </summary>
        public void Recul(int dir)
        {
            vitRecul = (int)Partie.ValeurSync(8);
            directionRecul = dir;
        }



        public override void Actualiser()
        {
            // Recul
            ResetPrevious();

            if (vitRecul > 0)
            {

                vitRecul -= vitRalentiRecul;
                if (vitRecul < 0) { vitRecul = 0; }

                Deplacer(vitRecul, directionRecul);
            }
        }
    }
}
