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

        private enum FunctionIdentifier : byte
        {
            READ_PROCESS_MEMORY,
            WRITE_PROCESS_MEMORY
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RemoteCallInformation
        {
            public FunctionIdentifier functionIdentifier;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = payloadSize)]
            public byte[] payload;
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

        public T ReadProcessMemory<T>(IntPtr lpBaseAddress, uint dwSize)
        {
            ByteStack callStack = new ByteStack(payloadSize);

            callStack.Push(getBytes(lpBaseAddress));
            callStack.Push(getBytes(dwSize));

            RemoteCallInformation remoteCallInfo = new RemoteCallInformation();
            remoteCallInfo.functionIdentifier = FunctionIdentifier.READ_PROCESS_MEMORY;
            remoteCallInfo.payload = callStack.ToArray();

            //send to client
            //receive answer
            byte[] result = new byte[101];
            return fromBytes<T>(result);
        }
    }
}
