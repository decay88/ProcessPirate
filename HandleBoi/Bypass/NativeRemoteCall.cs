using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HandleBoi
{
    class NativeRemoteCall
    {
        private const int payloadSize = 100;
        private readonly SocketServer socket; 
        private List<byte> bytesRec = new List<byte>();
        private readonly CallbackWatcher callbackWatcher;

        private enum FunctionIdentifier : byte
        {
            READ_PROCESS_MEMORY,
            WRITE_PROCESS_MEMORY,
            GET_PROC_ADDRESS,
            OPEN_PROCESS,
            GET_MODULE_BASE
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RemoteCallInformation
        {
            public FunctionIdentifier functionIdentifier;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = payloadSize)]
            public byte[] payload;
        }

        public NativeRemoteCall(ref SocketServer socket, int callbackTimeout)
        {
            this.socket = socket;
            callbackWatcher = new CallbackWatcher(socket, callbackTimeout);
        }

        private byte[] getBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        T fromBytes<T>(byte[] arr)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return obj;
        }

        public T ReadProcessMemory<T>(IntPtr lpBaseAddress)
        {
            ByteStack callStack = new ByteStack(payloadSize);

            callStack.Push(getBytes(lpBaseAddress));
            callStack.Push(getBytes(Marshal.SizeOf(typeof(T))));

            RemoteCallInformation remoteCallInfo =
                new RemoteCallInformation
                {
                    functionIdentifier = FunctionIdentifier.READ_PROCESS_MEMORY,
                    payload = callStack.GetBytes()
                };
            //send to client
            //receive answer
            byte[] result = callbackWatcher.SendAndWaitForCallback(getBytes(remoteCallInfo));
            return fromBytes<T>(result);
        }

        public IntPtr GetProcAddress(IntPtr hModule, string procName)
        {
            ByteStack callStack = new ByteStack(payloadSize);

            callStack.Push(getBytes(hModule));
            callStack.Push(Encoding.ASCII.GetBytes(procName));

            RemoteCallInformation remoteCallInfo =
                new RemoteCallInformation
                {
                    functionIdentifier = FunctionIdentifier.GET_PROC_ADDRESS,
                    payload = callStack.GetBytes()
                };
            //send to client
            //receive answer
            byte[] result = callbackWatcher.SendAndWaitForCallback(getBytes(remoteCallInfo));
            return result == null ? IntPtr.Zero : fromBytes<IntPtr>(result);
        }

        //return saved in host
        public void OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId)
        {
            ByteStack callStack = new ByteStack(payloadSize);

            callStack.Push(getBytes(dwDesiredAccess));
            callStack.Push(getBytes(bInheritHandle));
            callStack.Push(getBytes(dwProcessId));

            RemoteCallInformation remoteCallInfo =
                new RemoteCallInformation
                {
                    functionIdentifier = FunctionIdentifier.OPEN_PROCESS,
                    payload = callStack.GetBytes()
                };
            //send to client
            socket.SendBytes(getBytes(remoteCallInfo));
        }

        public IntPtr GetModuleBase(string moduleName)
        {
            ByteStack callStack = new ByteStack(payloadSize);

            callStack.Push(Encoding.Unicode.GetBytes(moduleName));
            RemoteCallInformation remoteCallInfo =
                new RemoteCallInformation
                {
                    functionIdentifier = FunctionIdentifier.GET_MODULE_BASE,
                    payload = callStack.GetBytes()
                };

            byte[] result = callbackWatcher.SendAndWaitForCallback(getBytes(remoteCallInfo));
            return result == null ? IntPtr.Zero : fromBytes<IntPtr>(result);
        }
    }
}
