using System;
using System.Runtime.InteropServices;

using memu.util;

namespace memu.memory
{
    class MemoryFacade
    {

        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        //const int PROCESS_ALL_ACCESS = 0x1F0FFF; // ???

        private readonly Logger logger;

        internal MemoryFacade(Logger logger)
        {
            this.logger = logger;
        }


        internal IntPtr OpenProcess(int processId)
        {
            IntPtr processHandle = Win32Memory.OpenProcess(PROCESS_VM_READ
                | PROCESS_VM_WRITE
                | PROCESS_VM_OPERATION,
                false,
                processId);

            logger.Info($"Opened handle #{processHandle} for process #{processId}");

            return processHandle;
        }

        internal IntPtr ReadPointer(IntPtr handle, IntPtr pointer)
        {
            byte[] buffer = new byte[IntPtr.Size];
            int bytesRead = 0;

            Win32Memory.ReadProcessMemory(handle, pointer, buffer, IntPtr.Size, ref bytesRead);

            var value = BitConverter.ToInt64(buffer, 0);

            logger.Debug($"Returning pointer 0x{value:X}");

            return new IntPtr(value);
        }

        internal int ReadInt32(IntPtr handle, IntPtr pointer)
        {
            byte[] buffer = new byte[4];
            int bytesRead = 0;

            Win32Memory.ReadProcessMemory(handle, pointer, buffer, 4, ref bytesRead);

            var value = BitConverter.ToInt32(buffer, 0);

            logger.Debug($"Returning int value {value}");

            return value;
        }

        internal long ReadInt64(IntPtr handle, IntPtr pointer)
        {
            byte[] buffer = new byte[8];
            int bytesRead = 0;

            Win32Memory.ReadProcessMemory(handle, pointer, buffer, 8, ref bytesRead);

            var value = BitConverter.ToInt64(buffer, 0);

            logger.Debug($"Returning long value {value}");

            return value;
        }

        internal void WriteInt32(IntPtr handle, IntPtr pointer, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteValue(handle, pointer, value, buffer);
        }

        internal void WriteInt64(IntPtr handle, IntPtr pointer, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteValue(handle, pointer, value, buffer);
        }


        private void WriteValue(IntPtr handle, IntPtr pointer, object value, byte[] buffer)
        {
            int bytesWritten = 0;

            if (Win32Memory.WriteProcessMemory(handle, pointer, buffer, IntPtr.Size, ref bytesWritten))
            {
                logger.Debug($"Wrote value {value} to address {pointer}");
            }
        }
    }


    class Win32Memory
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(int dwDesiredAccess,
                                                  bool bInheritHandle,
                                                  int dwProcessId);

        [DllImport("kernel32.dll")]
        internal static extern bool ReadProcessMemory(IntPtr hProcess,
                                                      IntPtr lpBaseAddress,
                                                      byte[] lpBuffer,
                                                      int dwSize,
                                                      ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess,
                                                       IntPtr lpBaseAddress,
                                                       byte[] lpBuffer,
                                                       int dwSize,
                                                       ref int lpNumberOfBytesWritten);
    }
}
