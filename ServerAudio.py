import socket
import wave
import pyaudio
import commands

CHUNK = 8192
FORMAT = pyaudio.paInt16
CHANNELS = 1
RATE = 44100
WAVE_OUTPUT_FILENAME = "server_output.wav"
WIDTH = 2

#HOST = 'localhost'     # Symbolic name meaning all available interfaces
#HOST = '192.168.0.21'     # Symbolic name meaning all available interfaces
PORT = 50004            # Arbitrary non-privileged port

address_list = commands.getoutput('hostname -I').split()
if len(address_list) > 1:
    address = address_list[1]
else:
    address = address_list[0]
HOST = address

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(5)



p = pyaudio.PyAudio()
stream = p.open(format=p.get_format_from_width(WIDTH),
                channels=CHANNELS,
                rate=RATE,
                input=True,
                frames_per_buffer=CHUNK)
stream.start_stream()

def saveFile(byte):
    wf = wave.open('F:\\'+WAVE_OUTPUT_FILENAME, 'wb')
    wf.setnchannels(CHANNELS)
    wf.setsampwidth(p.get_sample_size(FORMAT))
    wf.setframerate(RATE)
    wf.writeframes(byte)
    wf.close()
def main():    
    while 1:
        conn, addr = s.accept()
        print('connected by ' + str(addr))
        print("*_>recording")
        while 1:
            try:
                data = stream.read(CHUNK)
                print(len(data),type(data))
                conn.sendall(data)
            except Exception as e:
                print(e)
                data = '\x00' * CHUNK
                conn.close()
                break;
        print("*_>done recording")
    
    stream.stop_stream()
    stream.close()
    p.terminate()
    conn.close()

if __name__ == '__main__':
    main()