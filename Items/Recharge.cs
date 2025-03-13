using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serveur_AFKSimulator.ObjectsMap;

namespace Serveur_AFKSimulator.Items
{
    public class Recharge : Item
    {
        // ------------------------------ Attributs ------------------------------


        private const int qteProjectiles = 5;


        // ------------------------------ Méthodes ------------------------------


        public Recharge(int id, int x, int y, Map map) : base(id, x, y, map)
        {

        }

        public override void Actualiser() { }

        public override string InfosItem() => string.Join(Client.sep4, [id.ToString(), x.ToString(), y.ToString()]) + Client.sep3;

        public override void RecupererItem(Joueur j)
        {
            if (Joueur.nbProjectilesMax > j.nbProjectiles + qteProjectiles)
            {
                j.nbProjectiles += qteProjectiles;
            }
            else
            {
                j.nbProjectiles = Joueur.nbProjectilesMax;
            }
            suppr = true;
        }
    }
}
