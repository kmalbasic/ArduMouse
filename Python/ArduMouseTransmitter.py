import select
import sys
import socket
import os
import threading
from threading import *
import time
import datetime
from pynput.mouse import Button, Controller

def calculate_delta_x(prev_x, curr_x):
    if(prev_x - curr_x) > 0 or (prev_x - curr_x) < 0:
        val = ((prev_x - curr_x) * (-1))
        return val
    else:
        return 0

def calculate_delta_y(prev_y, curr_y):
    if(prev_y - curr_y) > 0 or (prev_y - curr_y) < 0:
        val = ((prev_y - curr_y) * (-1))
        return val
    else:
        return 0

def listener(client, address):
    with clients_lock:
        clients.add(client)
    try:
        prev_x = 0
        prev_y = 0
        while True:
            #pyautogui.position(curr_x,curr_y)
            mouse = Controller()
            current_mouse_position = mouse.position
            curr_x = mouse.position[0]
            curr_y = mouse.position[1]
            delta_y = calculate_delta_y(prev_y, curr_y)
            delta_x = calculate_delta_x(prev_x, curr_x)
            print(delta_x, delta_y)
            prev_x, prev_y = curr_x, curr_y
            data = client.recv(1024)
            if data == b'0':
                msg = str(delta_x) + '.' + str(delta_y)
                client.send(msg.encode())
                time.sleep(1)
    finally:
        with clients_lock:
            clients.remove(client)
            client.close()

clients = set()
clients_lock = threading.Lock()

host = sys.argv[1]
port = sys.argv[2]

s = socket.socket()
s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
s.bind((host,int(port)))
s.listen(3)
th = []

while True:
    client, address = s.accept()
    th.append(Thread(target=listener, args = (client,address)).start())
s.close()






# send deltas to client
