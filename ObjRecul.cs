using AFKSimulator;
using System.ComponentModel;


namespace Serveur_AFKSimulator
{
    public class ObjRecul : ElementMap
    {
        public int vitesse { get; set; }
        public int ralentit { get; set; }
        public int direction { get; set; }


        public ObjRecul(int id, int x, int y) : base(id, x, y)
        {
            ralentit = Partie.VitesseSync(1);
            vitesse = 0;
            direction = 0;
        }

        public void Recul(int dir)
        {
            vitesse = Partie.VitesseSync(8);
            direction = dir;
        }

        public override void Actualiser() { }

        public void ActualiserRecul(Map map)
        {
            if (vitesse > 0)
            {
                
                vitesse -= ralentit;
                if (vitesse < 0) { vitesse = 0; }

                int x2 = x+DepX(vitesse, direction);
                int y2 = y+DepY(vitesse, direction);

                Console.WriteLine(x2.ToString() + "   " + y2.ToString());

                if (DepPossible(map, x2, y2))
                {
                    x = x2;
                    y = y2;
                }
            }
        }
    }
}
