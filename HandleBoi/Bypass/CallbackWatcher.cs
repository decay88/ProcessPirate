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
        private readonly int timeout;
        private readonly SocketServer server;
        private bool receivedNewData = false;
        private byte[] result;
        private Stopwatch stopwatch = new Stopwatch();

        private void onMessageReceive(byte[] data, int length)
        {
            result = data;
            receivedNewData = true;
        }

        public CallbackWatcher(SocketServer server, int timeout)
        {
            this.timeout = timeout;
            this.server = server;
            server.onMessageReceiveCallback += onMessageReceive;
        }

        public byte[] SendAndWaitForCallback(byte[] data, int timeout = 0)
        {
            result = null;
            receivedNewData = false;
            stopwatch.Reset();
            stopwatch.Start();
            server.SendBytes(data);
            while (!receivedNewData)
            {
                if(stopwatch.ElapsedMilliseconds > (timeout > 0 ? timeout : this.timeout))
                    throw new Exception("function callback timeout!");
            }
            stopwatch.Stop();
            return result;
        }
    }
}
