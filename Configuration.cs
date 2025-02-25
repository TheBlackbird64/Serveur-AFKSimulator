using Serveur_AFKSimulator.ObjectsMap;
using Serveur_AFKSimulator.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serveur_AFKSimulator
{

    // Classes pour ranger les variables dans le fichier (Par classe, il faut une classe ici avec le nom Config+nomdelaclasse)
    public class ConfigReseau
    {
        public int maxConnexions { get; set; }
        public int port { get; set; }
        public bool ipv4 { get; set; }
    }

    public class ConfigPartie
    {
        public int nbJoueursMin { get; set; }
        public int nbJoueursMax { get; set; }
        public int actualiserIntervalleMs { get; set; }
        public int tempsVictoire { get; set; }
        public int tempsSupprPartie { get; set; }
    }

    public class ConfigClient
    {
        public int tempsSupprClient { get; set; }
    }

    public class ConfigJoueur
    {
        public int nbProjectilesMax { get; set; }
        public int tempsRechargeMax { get; set; }
        public int vieMax { get; set; }
        public int larg { get; set; }
        public int haut { get; set; }
    }

    public class ConfigOrbeCouleur
    {
        public int valAjoutCouleur { get; set; }
    }



    public class Configuration
    {
        public static string nameFile = "config.json";
        public static string versionConfigStatic = "1.0.0";


        public string versionConfig { get; set; } = "0.0.0";

        // Ajouter un champ du type de chaque classe
        public ConfigReseau configReseau { get; set; } = new();
        public ConfigPartie configPartie { get; set; } = new();
        public ConfigClient configClient { get; set; } = new();
        public ConfigJoueur configJoueur { get; set; } = new();
        public ConfigOrbeCouleur configOrbeCouleur { get; set; } = new();



        public static void LoadConfig()
        {
            Console.WriteLine("Version requise du fichier de configuration : " + versionConfigStatic + "\n");

            if (!File.Exists(nameFile))
            {
                Console.WriteLine("--> Aucune configuration trouvée. Editer le fichier " + nameFile + ".\n");
                CreateConfig();
                Environment.Exit(0);
            }

            string json = File.ReadAllText(nameFile);
            Configuration? config = JsonSerializer.Deserialize<Configuration>(json);

            if (config == null || config.versionConfig != versionConfigStatic)
            {
                Console.WriteLine("--> Erreur de configuration. Editer le fichier " + nameFile + ".\n");
                Environment.Exit(0);
            }

            // Assignation des variables static avec les valeurs du fichier json

            // Serveur
            Serveur.maxConnexions = config.configReseau.maxConnexions;
            Serveur.port = config.configReseau.port;
            Serveur.ipv4 = config.configReseau.ipv4;

            // Partie
            Partie.nbJoueursMin = config.configPartie.nbJoueursMin;
            Partie.nbJoueursMax = config.configPartie.nbJoueursMax;
            Partie.actualiserIntervalleMs = config.configPartie.actualiserIntervalleMs;
            Partie.tempsVictoire = config.configPartie.tempsVictoire;
            Partie.tempsSupprPartie = config.configPartie.tempsSupprPartie;

            // Client
            Client.tempsSupprClient = config.configClient.tempsSupprClient;

            // Joueur
            Joueur.nbProjectilesMax = config.configJoueur.nbProjectilesMax;
            Joueur.tempsRechargeMax = config.configJoueur.tempsRechargeMax;
            Joueur.vieMax = config.configJoueur.vieMax;
            Joueur.larg = config.configJoueur.larg;
            Joueur.haut = config.configJoueur.haut;

            // Items

            // orbeCouleur
            OrbeCouleur.valAjoutCouleur = config.configOrbeCouleur.valAjoutCouleur;

            Console.WriteLine("Réglages serveur: ");
            Console.Write("- IP");
            Console.Write(config.configReseau.ipv4 ? "v4" : "v6");
            Console.WriteLine("\n- Port: " + config.configReseau.port.ToString() + "\n");
        }

        public static void CreateConfig()
        {
            Configuration config = new Configuration();
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(nameFile, json);
        }
    }
}
