#!/usr/bin/python
import socket
import RPi.GPIO as GPIO
#import time
import commands

MODE_MIN = 0
MODE_MAX = 1

GPIO.setmode(GPIO.BOARD)
GPIO.setwarnings(False)
GPIO.setup(11, GPIO.OUT)
GPIO.setup(12, GPIO.OUT)
GPIO.setup(16, GPIO.OUT)
GPIO.setup(15, GPIO.OUT)
GPIO.setup(18, GPIO.OUT)
GPIO.setup(22, GPIO.OUT)

GPIO.setup(13, GPIO.OUT)
CC = GPIO.PWM(13, 50)
CC.start(0)

mode = MODE_MIN

def left():
    GPIO.output(11, 1)
    GPIO.output(12, 0)

def right():
    GPIO.output(12, 1)
    GPIO.output(11, 0)
    
def up():
    GPIO.output(15, 1)
    GPIO.output(16, 0)
    
def down():
    GPIO.output(16, 1)
    GPIO.output(15, 0)
        
def stop():
    GPIO.output(11, 0)
    GPIO.output(12, 0)
    GPIO.output(15, 0)
    GPIO.output(16, 0)
        
def left1():
    CC.ChangeDutyCycle(2.5)

def right1():
    CC.ChangeDutyCycle(12.5)

def stop1():
    CC.ChangeDutyCycle(7.5)     

def setMode(md):
    if md == MODE_MIN:
        GPIO.output(18, 1)
        GPIO.output(22, 0)
    else:
        GPIO.output(18, 0)
        GPIO.output(22, 1)

s = socket.socket()

#address= "192.168.0.101"
address_list = commands.getoutput('hostname -I').split()
if len(address_list) > 1:
    address = address_list[1]
else:
    address = address_list[0]
#address = "192.168.0.21"
port = 5007
print( 'Address: ', address, 'Port: ' + str(port))
s.bind((address, port))
s.listen(5)
    
try:
    stop1()
    while 1:
        conn, addr = s.accept()
        print('Connection address: ', addr)
        try:
            while 1: 
                data = conn.recv(1024)
                if not data : break
                print('received data: ', data)
                if data.startswith(b'close'):
                    stop()
                    break
                elif data.startswith(b'up2'):
                    up()
                elif data.startswith(b'down2'):
                    down()
                elif data.startswith(b'left2'):
                    left()
                elif data.startswith(b'right2'):
                    right()
                elif data.startswith(b'stop2'):
                    stop()
                elif data.startswith(b'min'):
                    setMode(MODE_MIN)
                elif data.startswith(b'max'):
                    setMode(MODE_MAX)
                elif data.startswith(b'left1'):
                    left1()
                elif data.startswith(b'right1'):
                    right1()
                elif data.startswith(b'stop1'):
                    stop1()
            conn.close()
            #break
        except socket.error as e:
            print("Socket Error:", e)
            #conn.close()
            #pass
except KeyboardInterrupt:
    print("Interrupted by user")

finally:
    stop()
    CC.stop()    
    GPIO.cleanup()
