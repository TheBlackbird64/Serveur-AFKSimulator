using Math;

public class Projectile
{
    public int idJoueur { get; set; }
    public int idProjectile { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int direction { get; set; }
    public int vitesse = 5;


    public Projectile(int x, int y, int idJoueur, int idProjectile, int direction)
    {
        this.x = x;
        this.y = y;
        this.idJoueur = idJoueur;
        this.idProjectile = idProjectile;
        this.direction = direction;
    }

    public ActualiserProjectile()
    {
        x = x + Math.Cos(direction) * vitesse;
        y = y + Math.Sin(direction) * vitesse;
    }


}