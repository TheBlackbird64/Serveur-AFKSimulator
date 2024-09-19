using System;
using System.Net.Sockets;
using System.Text;



public class Client
{

    private Socket sock;
    private Serveur serveur;
    public Partie? partie;
    

    public int id { get; set; }
    public String pseudo { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int vie { get; set; }
    public String couleur { get; set; }

    public const String sep1 = "|";
    public const String sep2 = ",";
    public const String sep3 = ";";


    public Client(Socket socket, int _id, Serveur serv)
    {
        
        sock = socket;
        serveur = serv;

        id = _id;
        pseudo = "";
        x = 0;
        y = 0;
        vie = 0;
        couleur = "000000";

        Log("Client connecté");
    }

    public async void RecMessagesAsync()
    {
        bool connecte = true;

        while (connecte)
        {
            byte[] buffer = new byte[1024];
            string message = "";

            try
            {
                int bytesRead = 1024;
                while (bytesRead == 1024)
                {
                    bytesRead = 0;
                    bytesRead = await sock.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead > 0)
                    {
                        message += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        
                    }
                    else
                    {
                        Log("Client deconnecte");
                        connecte = false;
                    }
                }

                String[] msgTab = message.Split(sep1);
                foreach (string msg in msgTab)
                {
                    TraiterMessages(msg.Split(sep2));
                }
            }
            catch (Exception e)
            {
                Log("Client deconnecte suite a une erreur: " + e.Message);
                connecte = false;
            }
        }

        serveur.clientListe.Remove(this);
        Log(serveur.clientListe.Count().ToString());
    }

    public void TraiterMessages(String[] msg)
    {
        if (msg[0] == "j")
        {
            pseudo = msg[1];
            Partie.fileAttente.Add(this);
        }
        else if (msg[0] == "a")
        {

        }
    }

    public void EnvoyerMessage(String msg)
    {
        sock.Send(Encoding.UTF8.GetBytes(msg + sep1));
    }

    public void Log(String msg)
    {
        Console.WriteLine("  " + msg + " (id:" + id.ToString() + ")");
    }
}