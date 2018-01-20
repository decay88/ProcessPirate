using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HandleBoi
{
    static class DLLinjection
    {
        public static bool LoadLibraryA(this Process targetProcess, string dllName)
        {
            IntPtr procHandle = targetProcess.Handle; // NativeImports.OpenProcess(NativeImports.Flags.HandlePrivileges.PROCESS_ALL_ACCESS, false, targetProcess.Id);
            IntPtr loadLibraryAddr = NativeImports.GetProcAddress(NativeImports.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            IntPtr allocMemAddress = NativeImports.VirtualAllocEx(
                procHandle,
                IntPtr.Zero,
                (uint)dllName.Length + 1, //(UInt32) ((dllName.Length + 1) * Marshal.SizeOf(typeof(byte))),
                NativeImports.Flags.MemoryPrivileges.MEM_COMMIT | NativeImports.Flags.MemoryPrivileges.MEM_RESERVE,
                NativeImports.Flags.MemoryPrivileges.PAGE_READWRITE
            );

            UIntPtr bytesWritten;
            Console.WriteLine(NativeImports.WriteProcessMemory(
                procHandle,
                allocMemAddress,
                Encoding.Default.GetBytes(dllName),
                (UInt32) (dllName.Length + 1),
                out bytesWritten
            ));

            // creating a thread that will call LoadLibraryA with allocMemAddress as argument
            NativeImports.CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

            return true;
        }
    }
}
