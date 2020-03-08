using System;
using System.Diagnostics;
using System.Threading;

using memu.memory;
using memu.util;

namespace memu
{
    public class MemU
    {
        public static bool VerboseLogging { get; set; } = false;

        public IntPtr ProcessBaseAddress { get; set; }
        string ProcessName { get; set; }
        int ProcessId { get; set; }
        IntPtr ProcessHandle { get; set; }



        readonly string requestedProcess;
        public MemU(string requestedProcess)
        {
            this.requestedProcess = requestedProcess;
        }

        /// <summary>
        /// Attempts to attach to requested process.
        /// Waits till process with given name is started.
        /// </summary>
        public void OpenProcess()
        {
            LOG.Info($"Waiting for a process {requestedProcess}...");

            WaitForProcess(out Process process);

            GatherProcessInfo(process);

            LOG.Info($"Found process; name - {ProcessName}, id - {ProcessId}, base address - 0x{ProcessBaseAddress.ToInt64():X}");

            ProcessHandle = MemoryFacade.OpenProcess(ProcessId);
        }

        /// <summary>
        /// Reads and returns address pointer at given address.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Address pointer.</returns>
        public IntPtr ReadPointer(IntPtr target)
        {
            return MemoryFacade.ReadPointer(ProcessHandle, target);
        }

        /// <summary>
        /// Reads and returns address pointer at given address applying given offsets.
        /// First offset applies to argument pointer, second offset applies to returned pointer and so on.
        /// </summary>
        /// <param name="pointer">Base address pointer</param>
        /// <param name="offsets">Chain of offsets</param>
        /// <returns></returns>
        public IntPtr ReadPointer(IntPtr start, params int[] offsets)
        {
            var ptr = start;

            foreach (int offset in offsets)
            {
                ptr = MemoryFacade.ReadPointer(ProcessHandle, IntPtr.Add(ptr, offset));
            }

            return ptr;
        }

        // TODO: add summary
        public int ReadInt32(IntPtr target)
        {
            return MemoryFacade.ReadInt32(ProcessHandle, target);
        }

        // TODO: add summary
        public long ReadInt64(IntPtr target)
        {
            return MemoryFacade.ReadInt64(ProcessHandle, target);
        }

        // TODO: add summary
        public void WriteInt32(IntPtr target, int value)
        {
            MemoryFacade.WriteInt32(ProcessHandle, target, value);
        }

        // TODO: add summary
        public void WriteInt64(IntPtr target, long value)
        {
            MemoryFacade.WriteInt64(ProcessHandle, target, value);
        }

        // TODO: add summary
        private void WaitForProcess(out Process process)
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName(requestedProcess);
                if (processes == null || processes.Length == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                process = processes[0];

                break;
            }
        }

        // TODO: add summary
        private void GatherProcessInfo(Process process)
        {
            ProcessName = process.ProcessName;
            ProcessId = process.Id;
            ProcessBaseAddress = process.MainModule.BaseAddress;
        }
    }
}
