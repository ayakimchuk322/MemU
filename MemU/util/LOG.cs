using System;
using System.Threading;

namespace memu.util
{
    class LOG
    {

        public static void Debug(string message)
        {
            if (MemU.VerboseLogging)
            {
                DateTime now = DateTime.Now;
                string name = Thread.CurrentThread.Name;

                Console.WriteLine($"DEBUG | {now} | {name} | " + message);
            }
        }

        public static void Info(string message)
        {
            DateTime now = DateTime.Now;
            string name = Thread.CurrentThread.Name;

            Console.WriteLine($"INFO | {now} | {name} | " + message);
        }
    }
}
