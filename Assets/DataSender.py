from ctypes import sizeof
import socket
import pickle
import json
import time

PORT = 5005 #Port number
SERVER = socket.gethostbyname(socket.gethostname())
ADDR = (SERVER, PORT)

client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
client.connect(ADDR)


# Takes a message and serializes it by first converting it into a Json and then a byte array
# The message is then sent to the reciever
def send(msg):
    if(isinstance(msg, str)):
        message = pickle.dumps(msg)
    else:
        message = pickle.dumps(json.dumps(msg))
    client.send(message)

# Parses the given file and its data
def openFile(filename: str):
    file = open(filename, 'r')
    dictionaries = file.read().split("|\n")
    for dict in dictionaries:
        send(dict)
        time.sleep(1/60)
    file.close()


openFile("Dictionary Samples\Random1000.txt")
