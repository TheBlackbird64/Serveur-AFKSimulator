
public class Projectile : ElementMap
{
    public int idJoueur { get; set; }
    public int direction { get; set; }
    public int vitesse { get; set; } = 5;
    public int degats { get; set; } = 5;
    public Partie partie { get; set; }


    public Projectile(int id, int x, int y, int idJoueur, int direction, Partie partie) : base(id, x, y)
    {
        this.idJoueur = idJoueur;
        this.direction = direction;
        this.partie = partie;
    }

    public override void Actualiser()
    {
        
        x = x + Convert.ToInt32(Math.Round(Math.Cos((double) direction * (Math.PI / 180.0)))) * vitesse;
        y = y + Convert.ToInt32(Math.Round(Math.Sin((double) direction * (Math.PI / 180.0)))) * vitesse;

        // On gère les dégats quand il y a une collision avec un des joueurs, sauf celui qui a lancé le projectile sinon il est touché à la création du projectile (variable idJoueur)
        foreach (Joueur j in partie.listeJoueurs)
        {
            if (idJoueur != j.id)
            {
                if (Collision(this, j))
                {
                    j.vie -= degats;
                    SupprimerProjectile();
                    break;
                }
            }
        }
    }

    public void SupprimerProjectile()
    {
        partie.listeProjectile.Remove(this);
    }
}