using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serveur_AFKSimulator.Items
{
    public class Recharge : Item
    {
        private const int qteProijectiles = 5;

        public Recharge(int id, int x, int y, Map map) : base(id, x, y, map)
        {
            largeur = 30;
            hauteur = 30;
        }

        public override void Actualiser() { }

        public override string InfosItem() => string.Join(Client.sep4, [id.ToString(), x.ToString(), y.ToString()]) + Client.sep3;

        public override void RecupererItem(Joueur j)
        {
            if (Joueur.nbProjectilesMax > j.nbProjectiles + qteProijectiles)
            {
                j.nbProjectiles += qteProijectiles;
            }
            else
            {
                j.nbProjectiles = Joueur.nbProjectilesMax;
            }
            suppr = true;
        }
    }
}
