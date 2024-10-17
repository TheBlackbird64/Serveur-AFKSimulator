using AFKSimulator;
using System.Diagnostics;

namespace Serveur_AFKSimulator.Items
{
    public class OrbeCouleur : Item
    {
        public static int maxItems = 8;
        public static int delaiApparition = 3;
        public static Stopwatch chrono = new Stopwatch();

        const int valAjoutCouleur = 17;
        public enum Couleur { Rouge, Vert, Bleu }
        Couleur col;

        public OrbeCouleur(int id, int x, int y) : base(id, x, y)
        {
            largeur = 30;
            hauteur = 30;

            if (! chrono.IsRunning) { chrono.Start(); }

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
            string ajoutCol = valAjoutCouleur.ToString("X");
            if (col == Couleur.Rouge )
            {
                j.colRouge += ajoutCol;
            }
            if (col == Couleur.Vert)
            {
                j.colVert += ajoutCol;
            }
            if (col == Couleur.Bleu)
            {
                j.colBleu += ajoutCol;
            }

            suppr = true;
        }
    }
}
