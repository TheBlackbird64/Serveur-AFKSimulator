using Serveur_AFKSimulator.ObjectsMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serveur_AFKSimulator
{

    // Classes pour ranger les variables ds le fichier (Par classe, il faut une classe ici avec le nom Config+nomdelaclasse)
    public class ConfigReseau
    {
        public int maxConnexions { get; set; }
        public int port { get; set; }
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



    public class Configuration
    {
        public static string nameFile = "config.json";

        // Ajouter un champ du type de chaque classe
        public ConfigReseau configReseau { get; set; } = new();
        public ConfigPartie configPartie { get; set; } = new();
        public ConfigClient configClient { get; set; } = new();
        public ConfigJoueur configJoueur { get; set; } = new();


        public static void LoadConfig()
        {

            if (!File.Exists(nameFile))
            {
                Console.WriteLine("--> Aucune configuration trouvée. Editer le fichier " + nameFile + ".\n");
                CreateConfig();
            }

            string json = File.ReadAllText(nameFile);
            Configuration? config = JsonSerializer.Deserialize<Configuration>(json);

            if (config == null)
            {
                Console.WriteLine("--> Erreur de configuration. Editer le fichier " + nameFile + ".\n");
                config = new Configuration();
                Environment.Exit(0);
            }

            // Assignation des variables static avec les valeurs du fichier json

            // Serveur
            Serveur.maxConnexions = config.configReseau.maxConnexions;
            Serveur.port = config.configReseau.port;

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


        }

        public static void CreateConfig()
        {
            Configuration config = new Configuration();

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(nameFile, json);
        }
    }
}
