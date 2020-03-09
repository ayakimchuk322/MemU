using System;
using System.Diagnostics;
using System.Threading;

using memu.memory;
using memu.util;

namespace memu
{
    public class MemU
    {
        public IntPtr ProcessBaseAddress { get; private set; }
        string ProcessName { get; set; }
        int ProcessId { get; set; }
        IntPtr ProcessHandle { get; set; }


        readonly string requestedProcess;
        readonly MemoryFacade memoryFacade;
        readonly Logger logger;


        internal MemU(string requestedProcess,
                      MemoryFacade memoryFacade,
                      Logger logger)
        {
            this.requestedProcess = requestedProcess;
            this.memoryFacade = memoryFacade;
            this.logger = logger;
        }


        /// <summary>
        /// Attempts to attach to requested process.
        /// Waits till process with given name is started.
        /// </summary>
        public void OpenProcess()
        {
            logger.Info($"Waiting for a process {requestedProcess}...");

            WaitForProcess(out Process process);

            GatherProcessInfo(process);

            logger.Info($"Found process; name - {ProcessName}, id - {ProcessId}, base address - 0x{ProcessBaseAddress.ToInt64():X}");

            ProcessHandle = memoryFacade.OpenProcess(ProcessId);
        }

        /// <summary>
        /// Reads and returns address pointer at given address.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Address pointer.</returns>
        public IntPtr ReadPointer(IntPtr target)
        {
            return memoryFacade.ReadPointer(ProcessHandle, target);
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
                ptr = memoryFacade.ReadPointer(ProcessHandle, IntPtr.Add(ptr, offset));
            }

            return ptr;
        }

        // TODO: add summary
        public int ReadInt32(IntPtr target)
        {
            return memoryFacade.ReadInt32(ProcessHandle, target);
        }

        // TODO: add summary
        public long ReadInt64(IntPtr target)
        {
            return memoryFacade.ReadInt64(ProcessHandle, target);
        }

        // TODO: add summary
        public void WriteInt32(IntPtr target, int value)
        {
            memoryFacade.WriteInt32(ProcessHandle, target, value);
        }

        // TODO: add summary
        public void WriteInt64(IntPtr target, long value)
        {
            memoryFacade.WriteInt64(ProcessHandle, target, value);
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


    public class MemUFactory
    {

        private static MemUFactory factory;


        private MemoryFacade memoryFacade;
        private Logger logger;


        public static MemUFactory GetFactory()
        {
            if (factory == null)
            {
                factory = new MemUFactory();
            }
            return factory;
        }


        public string ProcessName { get; set; }
        public bool VerboseLogging { get; set; }

        public MemU Build()
        {
            logger = new Logger(VerboseLogging);
            memoryFacade = new MemoryFacade(logger);

            MemU memu = new MemU(ProcessName, memoryFacade, logger);

            return memu;
        }
    }
}
