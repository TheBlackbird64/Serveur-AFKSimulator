
public class Projectile
{
    public int x { get; set; }
    public int y { get; set; }
    public int idJoueur { get; set; }
    public int idProjectile { get; set; }

    public Projectile(int x, int y, int idJoueur, int idProjectile)
    {
        this.x = x;
        this.y = y;
        this.idJoueur = idJoueur;
        this.idProjectile = idProjectile;
    }
}