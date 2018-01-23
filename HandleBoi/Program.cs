using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HandleBoi
{
    class Program
    {
        #if DEBUG
        private const String dllPath = @"D:\h4x\eigene\HandleBoi\x64\Debug\InterfaceDLL.dll";
#else
        private const String dllPath = @"D:\h4x\eigene\HandleBoi\x64\Release\InterfaceDLL.dll";
#endif

        private static void onMessageReceive(byte[] data, int length)
        {
        }

        // Incoming data from the client.  
        public static string data = "";

        private const String IP_ADDR = "127.0.0.1";
        private const int SERVER_PORT = 1337;
        private const int INPUT_BUFF_SIZE = 100;

        static void Main(string[] args)
        {
            Process.EnterDebugMode();
            int pid = Convert.ToInt32(Console.ReadLine());
            Process process =  Process.GetProcessById(pid);
            
            if (process == null)
            {
                throw new Exception("couldnt find process");
            }
            
            process.LoadLibraryA(dllPath);

            
            Process.LeaveDebugMode();

            SocketServer server = new SocketServer(IP_ADDR, SERVER_PORT, INPUT_BUFF_SIZE);
            server.Start(onMessageReceive);

            NativeRemoteCall remoteCall = new NativeRemoteCall(ref server, 1000);

            while (true)
            {
                Console.WriteLine("calling...");
                remoteCall.OpenProcess(
                    NativeImports.Flags.HandlePrivileges.PROCESS_ALL_ACCESS,
                    false,
                    6352
                );
                Console.ReadKey();
                int i = remoteCall.ReadProcessMemory<int>(new IntPtr(0x196CF96093C));
                Console.WriteLine(i);
            }
        }
    }
}
