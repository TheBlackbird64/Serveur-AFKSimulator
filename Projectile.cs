
namespace AFKSimulator
{
    public class Projectile : ElementMap
    {
        public int idJoueur { get; set; }
        public int direction { get; set; }
        public int vitesse { get; set; } = 40;
        public int degats { get; set; } = 10;
        public Partie partie { get; set; }
        public Map map { get; set; }


        public Projectile(int id, int x, int y, int idJoueur, int direction, Partie partie, Map map) : base(id, x, y)
        {
            this.idJoueur = idJoueur;
            this.direction = direction;
            this.partie = partie;
            largeur = 25;
            hauteur = 25;
            this.map = map;
        }

        public override void Actualiser()
        {

            x += Convert.ToInt32(Math.Round(Math.Cos(direction * Math.PI / 180.0) * vitesse));
            y += Convert.ToInt32(Math.Round(Math.Sin(-direction * Math.PI / 180.0) * vitesse));

            // On gère les dégats quand il y a une collision avec un des joueurs, sauf celui qui a lancé le projectile sinon il est touché à la création du projectile (variable idJoueur)
            foreach (Joueur j in partie.listeJoueurs)
            {
                if (idJoueur != j.id)
                {
                    if (Collision(this, j))
                    {
                        j.vie -= degats;
                        suppr = true;
                        break;
                    }
                }
            }

            // Suppression si sortie de la map
            if (x < Map.coinMapG || y < Map.coinMapH || Map.XToRow(x + largeur) >= Map.tailleMap || Map.YToRow(y + hauteur) >= Map.tailleMap) { suppr = true; }
            else
            {
                // Collisions
                if (Collision(this, map.TabBool()))
                {
                    suppr = true;
                }
            }

        }
    }
}
