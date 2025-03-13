

namespace Serveur_AFKSimulator.ObjectsMap
{
    public class Projectile : ElementMove
    {
        // ------------------------------ Attributs ------------------------------


        public int idJoueur { get; set; }
        public int direction { get; set; }
        public int degats { get; set; } = 20;
        public Partie partie { get; set; }


        // ------------------------------ Méthodes ------------------------------


        public Projectile(int id, int x, int y, int idJoueur, int direction, Partie partie, Map map) : base(id, x, y, map)
        {
            this.idJoueur = idJoueur;
            this.direction = direction;
            this.partie = partie;
            largeur = 25;
            hauteur = 25;
            vitesse = (int)Partie.ValeurSync(8);
        }


        public override void ObstacleTouche()
        {
            suppr = true;
        }

        public override void Actualiser()
        {

            Deplacer(vitesse, direction);

            // On gère les dégats quand il y a une collision avec un des joueurs, sauf celui qui a lancé le projectile sinon il est touché à la création du projectile (variable idJoueur)
            foreach (Client c in partie.listeClient)
            {
                if (c.joueur != null)
                {
                    Joueur j = c.joueur;
                    if (idJoueur != j.id)
                    {
                        if (Collision(this, j))
                        {
                            j.vie -= degats;
                            j.Recul(direction);
                            j.chrono.Restart();
                            suppr = true;
                            break;
                        }
                    }
                }
            }
        }

    }
}
