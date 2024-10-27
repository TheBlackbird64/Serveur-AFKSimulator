using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serveur_AFKSimulator
{
    // Cette classe sert à identifier les différents objets lorsqu'ils sont contenus dans des listes (ils ne peuvent pas être identifiés par leur positions car certains supprimés ou ajoutés souvent, et aussi pr le coté client)
    public class Identification
    {
        public int id { get; set; }

        // Trouve un id pas utilisé parmi une liste d'éléments ElementMap (id != 0)
        public static int TrouverIdDispo<T>(List<T> listeElem) where T : Identification
        {
            int idElem = 0;
            bool dejaPresent = true;
            while (dejaPresent)
            {
                idElem++;
                dejaPresent = false;
                foreach (T e in listeElem)
                {
                    if (e.id == idElem)
                    {
                        dejaPresent = true;
                    }
                }
            }

            return idElem;
        }
    }
}
