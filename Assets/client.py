import socket
import pickle
import json

PORT = 5005 #Port number
SERVER = socket.gethostbyname(socket.gethostname())
ADDR = (SERVER, PORT)

client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
client.connect(ADDR)


def send(msg):
    message = pickle.dumps(json.dumps(msg))
    client.send(message)

send({"hello": 5, "status": True, "name": "Thomas"})

