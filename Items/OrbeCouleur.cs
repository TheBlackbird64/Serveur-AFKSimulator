using Serveur_AFKSimulator.ObjectsMap;
using System.Diagnostics;

namespace Serveur_AFKSimulator.Items
{
    public class OrbeCouleur : Item
    {

        public static int valAjoutCouleur;
        public enum Couleur { Rouge, Vert, Bleu }
        public Couleur col;

        public OrbeCouleur(int id, int x, int y, Map map) : base(id, x, y, map)
        {
            largeur = 30;
            hauteur = 30;


            Random rnd = new Random();
            switch (rnd.Next(3))
            {
                case 0:
                    col = Couleur.Rouge; break;
                case 1:
                    col = Couleur.Vert; break;
                case 2:
                    col = Couleur.Bleu; break;
            }
        }

        public override void Actualiser() { }

        public override void RecupererItem(Joueur j)
        {
            if (col == Couleur.Rouge )
            {
                j.colRouge += valAjoutCouleur;
                if (j.colRouge > 255) { j.colRouge = 255; }
            }
            if (col == Couleur.Vert)
            {
                j.colVert += valAjoutCouleur;
                if (j.colVert > 255) { j.colVert = 255; }
            }
            if (col == Couleur.Bleu)
            {
                j.colBleu += valAjoutCouleur;
                if (j.colBleu > 255) { j.colBleu = 255; }
            }

            suppr = true;
        }

        public override string InfosItem() => string.Join(Client.sep4, [id.ToString(), x.ToString(), y.ToString(), ((int)col).ToString()]) + Client.sep3;
    }
}
