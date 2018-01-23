using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleBoi
{
    class CallbackWatcher
    {
        private int timeout;
        private bool receivedNewData = false;
        private byte[] result;
        private Stopwatch stopwatch = new Stopwatch();

        private void onMessageReceive(byte[] data, int length)
        {
            result = data;
            receivedNewData = true;
        }

        public CallbackWatcher(int timeout)
        {
            this.timeout = timeout;
        }

        public byte[] SendAndWaitForCallback(SocketServer server, byte[] data)
        {
            server.onMessageReceiveCallback += onMessageReceive;
            stopwatch.Reset();
            stopwatch.Start();
            server.SendBytes(data);
            result = null;
            while (!receivedNewData)
            {
                if(stopwatch.ElapsedMilliseconds > this.timeout)
                    throw new Exception("function callback timeout!");
            }
            stopwatch.Stop();
            server.onMessageReceiveCallback -= onMessageReceive;
            receivedNewData = false;
            return result;
        }
    }
}
