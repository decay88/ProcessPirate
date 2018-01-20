using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HandleBoi
{
    class SocketServer
    {
        private readonly IPEndPoint localEndPoint;
        private readonly Socket listener;
        private Socket handler;
        private Thread messageThread;
        private byte[] inputBuffer;

        public delegate void MessageReceiveCallback(byte[] data, int length);
        private MessageReceiveCallback onMessageReceiveCallback;

        public SocketServer(String ip, int port, int nInBufferSize)
        {
            inputBuffer = new byte[nInBufferSize];

            localEndPoint = new IPEndPoint(
                IPAddress.Parse(ip),
                port
            );

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);
        }

        private void MessageLoop()
        {
            Console.WriteLine("listening for messages...");

            while (true)
            {
                int bytesRecv = handler.Receive(inputBuffer);
                if (bytesRecv > 0)
                {
                    Console.WriteLine("got message.");
                    this.onMessageReceiveCallback(inputBuffer, bytesRecv);
                }
            }
        }

        public void Start(MessageReceiveCallback onMessageReceiveCallback)
        {
            if (messageThread != null)
            {
                throw new Exception("message loop already running");
            }

            this.onMessageReceiveCallback = onMessageReceiveCallback;

            Console.WriteLine("listening for clients...");
            handler = listener.Accept();
            Console.WriteLine("connection established.");

            messageThread = new Thread(MessageLoop);
            messageThread.Start();
        }

        public void SendBytes(byte[] data)
        {
            handler.Send(data);
        }

        public void SendString(String msg)
        {
            SendBytes(Encoding.ASCII.GetBytes(msg));
        }
    }
}
