import socket
import os
import sys
from threading import Thread
import time

s = socket.socket()  
host = sys.argv[1]       
port = sys.argv[2]

s.connect((host, int(port)))
while True:
    s.send(b'0')
    print(s.recv(1024))


# close the connection 
server.close()