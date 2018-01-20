using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HandleBoi
{
    class Pipe
    {
        private readonly NamedPipeServerStream pipeServerStream;
        private StreamReader pipeStreamReader;
        private StreamWriter pipeStreamWriter;
        private bool run = true;
        private Thread pipeLoopThread;

        public delegate void MessageReceiveCallback(String  data);
        private MessageReceiveCallback onMessageReceiveCallback;

        public Pipe(String name)
        {
            pipeServerStream = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message);
        }

        public bool IsConnected()
        {
            return pipeServerStream.IsConnected;
        }

        public void SendMessage(String data)
        {
            SendMessage(data.ToCharArray());
        }

        public void SendMessage(char[] data)
        {
            pipeStreamWriter.Write(data);
        }

        private void pipeLoop()
        {
            while (run)
            {
                String dataRead = pipeStreamReader.ReadLine();
                if (string.IsNullOrEmpty(dataRead))
                    this.onMessageReceiveCallback(dataRead);
            }
        }

        public void Start(MessageReceiveCallback onMessageReceiveCallback)
        {
            this.onMessageReceiveCallback = onMessageReceiveCallback;
            if (pipeLoopThread != null && pipeLoopThread.IsAlive)
                throw new Exception("illegal state. startup interrupted");

            run = true;
            pipeLoopThread = new Thread(this.pipeLoop);
            pipeLoopThread.Start();
        }

        public void Stop()
        {
            run = false;
            pipeLoopThread.Abort();
        }

        public void Connect()
        {
            Console.WriteLine("starting server...");
            pipeServerStream.WaitForConnection();
            pipeServerStream.Flush();
            pipeStreamReader = new StreamReader(pipeServerStream);
            pipeStreamWriter = new StreamWriter(pipeServerStream);
            Console.WriteLine("setup\n");
        }
    }
}
