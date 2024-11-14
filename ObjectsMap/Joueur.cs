
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using Serveur_AFKSimulator.Items;
using System.Linq;

namespace Serveur_AFKSimulator.ObjectsMap
{
    public class Joueur : ElementMove
    {
        public static int nbProjectilesMax = 10;
        public static int tempsRechargeMax = 1000;
        public static int vieMax = 100;
        public static int larg = 50;
        public static int haut = 50;


        public Partie partie;

        public string pseudo { get; set; }
        public int vie { get; set; }
        public int colRouge { get; set; }
        public int colVert { get; set; }
        public int colBleu { get; set; }
        public string couleur { get { return ColToString(colRouge) + ColToString(colVert) + ColToString(colBleu); } }
        public long tempsAfkMs { get { return chrono.ElapsedMilliseconds; } }
        public Stopwatch chrono { get; set; }
        private Stopwatch chronoTempsRecharge { get; set; }
        public int nbProjectiles { get; set; }

        private bool mouvH { get; set; } = false;
        private bool mouvB { get; set; } = false;
        private bool mouvD { get; set; } = false;
        private bool mouvG { get; set; } = false;
        private double vitesseH { get; set; } // vitesse verticale
        private double vitesseV { get; set; } // vitesse horizontale
        private double acceleration { get; set; } // acceleration du joueur (pour que le mouvement soit fluide)
        private double deceleration { get; set; } // deceleration du joueur (pour que le mouvement soit fluide)


        public static string ColToString(int col)
        {
            string c = col.ToString("X");
            if (c.Length == 1) { c = "0" + c; }
            return c;
        }


        public Joueur(Partie partie, int _id, string pseudo) : base(_id, 0, 0, partie.map)
        {
            Random rnd = new Random();
            vie = vieMax;
            nbProjectiles = nbProjectilesMax;

            colRouge = 0;
            colVert = 0;
            colBleu = 0;

            largeur = larg;
            hauteur = haut;
            x = Map.coinMapG - 25;
            y = Map.coinMapH - 25;

            while (Collision(this, map.tabBool))
            {
                x = rnd.Next(Map.coinMapG, Map.tailleMap * Map.tailleCellMap);
                y = rnd.Next(Map.coinMapH, Map.tailleMap * Map.tailleCellMap);
            }

            vitesse = (int) Partie.ValeurSync(5);
            acceleration = Partie.ValeurSync(0.1);
            deceleration = 0.75;
            vitesseH = 0;
            vitesseV = 0;

            chrono = new Stopwatch();
            chronoTempsRecharge = new Stopwatch();
            this.partie = partie;
            this.pseudo = pseudo;
            chrono.Start();
            chronoTempsRecharge.Start();
        }

        public void TraiterMessages(string[] msg)
        {
            if (msg[0] == "a") // Actualiser position et �ventuellement tir
            {
                bool err = false;
                int dirProjectile = 0;

                try
                {
                    dirProjectile = int.Parse(msg[5]);
                }
                catch { err = true; }

                if (!err)
                {
                    // Si le joueur bouge son chrono est remis � 0 (ou si il re�oit un projectile)
                    if (msg[1] != "0" || msg[2] != "0" || msg[3] != "0" || msg[4] != "0")
                    {
                        chrono.Restart();
                    }

                    // Deplacement (enregistrer dans des variables et apr�s le faire dans Actualiser sinon �a d�pendrait de la vitesse d'envoi du client)
                    mouvH = msg[1] != "0"; // haut
                    mouvB = msg[2] != "0"; // bas
                    mouvD = msg[3] != "0"; // droite
                    mouvG = msg[4] != "0"; // gauche

                    // Si msg[2] n'est pas �gal � -1, c'est que cette valeur est la direction du projectile tir�
                    // On cr�e le projectile aux coordonn�s du joueur avec la direction indiqu�e dans le message
                    if (msg[5] != "-1" && chronoTempsRecharge.ElapsedMilliseconds >= tempsRechargeMax && nbProjectiles > 0)
                    {
                        nbProjectiles--;
                        chronoTempsRecharge.Restart();
                        partie.listeProjectile.Add(new Projectile(TrouverIdDispo(partie.listeProjectile), x, y, id, dirProjectile, partie, partie.map));
                    }
                }
            }
        }

        public override void ObstacleTouche()
        {
            // Qd un obstacle est touch� on revient � la position pr�c�dente
            x = xPrevious;
            y = yPrevious;
        }

        public override void Actualiser()
        {
            if (mouvH) // haut
            {
                vitesseV -= acceleration;
                if (vitesseV < -1) { vitesseV = -1; }
            }
            if (mouvB) // bas
            {
                vitesseV += acceleration;
                if (vitesseV > 1) { vitesseV = 1; }
            }
            if (mouvD) // droite
            {
                vitesseH += acceleration;
                if (vitesseH > 1) { vitesseH = 1; }
            }
            if (mouvG) // gauche
            {
                vitesseH -= acceleration;
                if (vitesseH < -1) { vitesseH = -1; }
            }

            if (!(mouvH || mouvB))
            {
                vitesseV *= deceleration;
                if (Math.Abs(vitesseV) < 0.025) { vitesseV = 0; }
            }
            if (!(mouvG || mouvD))
            {
                vitesseH *= deceleration;
                if (Math.Abs(vitesseH) < 0.025) { vitesseH = 0; }
            }

            DeplacerX((int)(vitesseH * vitesse));
            DeplacerY((int)(vitesseV * vitesse));

            base.Actualiser();

            // Verification recuperer item
            // On v�rifie dans le tableau si un touch�, si c'est le cas pour savoir lequel on doit parcourir toute la liste
            if (Collision(this, Item.tabPosItem))
            {
                for (int i = 0; i < GestionnaireItem.tabTypes.Length; i++)
                {
                    for (int j = 0; j < GestionnaireItem.dictItemInstance[GestionnaireItem.tabTypes[i]].Count; j++)
                    {
                        if (Collision(this, GestionnaireItem.dictItemInstance[GestionnaireItem.tabTypes[i]][j]))
                        {
                            GestionnaireItem.dictItemInstance[GestionnaireItem.tabTypes[i]][j].RecupererItem(this);
                        }
                    }
                }
            }


            if (vie <= 0)
            {
                chrono.Stop();
                x = 0;
                y = 0;
            }
        }
    }
}
