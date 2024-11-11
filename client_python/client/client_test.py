import socket
import sys
import time
import random
from select import *

class ClientV3:
    def __init__(self, allow_debug=True, allow_logs=False, timeout_msg=0.001):
        # debug
        if allow_debug:
            f_debug = open("erreurs_client.txt", "w")
            sys.stderr = f_debug
        if allow_logs:
            f_logs = open("logs_client.txt", "w")
            f_logs.close()
        
        self.host = "services-afksimulator.alwaysdata.net"
        self.port = 8300
        self.liste_msg = []
        self.caractere_sep_infos = ","
        self.caractere_sep_msg = "|"
        self.timeout_msg = timeout_msg
        self.allow_logs = allow_logs
    
    def connexion(self):
        try:
            self.connection = socket.socket(socket.AF_INET6, socket.SOCK_STREAM)
            self.connection.settimeout(5)
            self.connection.connect((self.host, self.port))
        except:
            return False
        else:
            return True
        
    def lire_message(self, type="all"):
        # cette fonction renvoie tout les messages reçus d'un certain type (le type du message est la première information de celui ci)
        requetes, wlist, xlist = select([self.connection], [], [], self.timeout_msg)
        for req in requetes:
            try:
                data = req.recv(1024).decode()
            except ConnectionResetError:
                return -1
            else:
                for msg in data.split(self.caractere_sep_msg):
                    if msg != "":
                        self.liste_msg.append(msg)
        
        if type != "all":
            liste_msg_retour = []
            for msg in self.liste_msg:
                if msg.split(self.caractere_sep_infos)[0] == type:
                    liste_msg_retour.append(msg)
        else:
            liste_msg_retour = list(self.liste_msg)
        
        # on supprime tout les messages qu'on va retourner de la liste car ils ont déjà été lus (pour éviter qu'on les lise à chaque fois qu'on appelle la fonction) 
        for msg in liste_msg_retour:
            self.liste_msg.remove(msg)
            if self.allow_logs:
                with open("logs_client.txt", "a") as f_logs:
                    f_logs.write("reçu: " + msg + "\n")
        
        return liste_msg_retour
    
    def envoi_message(self, msg_tab):
        self.connection.send((self.caractere_sep_infos.join(msg_tab) + self.caractere_sep_msg).encode())


if __name__ == "__main__":
    c = ClientV3()
    while not c.connexion():
        print("Tentative de connexion.. ")
    print("\n  Connexion établie")
    c.envoi_message(["j","test"])
    
    b = True
    tmp = ""
    while b:
        tmp = c.lire_message()
        if len(tmp) > 0:
            if tmp[0][0] == "p":
                b = False
    
    c.envoi_message(["r"])
    x = random.randint(500, 2000)
    y = random.randint(500, 2000)
    
    p = time.time()
    while True:
        m = "-1"
        
        #x += random.randint(-2, 2)
        #y += random.randint(-2, 2)
        
        if (time.time() - p > 3):
            p = time.time()
            if (random.randint(0, 5) == 5):
                m = "45"
            
            
        c.envoi_message(["a", str(x), str(y), m])
        
        a = c.lire_message()
        if a != []:
            print(a)
        if a == -1:
            quit()
            exit()
