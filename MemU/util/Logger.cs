using System;
using System.Threading;

namespace memu.util
{
    class Logger
    {

        private bool verboseLogging;

        internal Logger(bool verboseLogging)
        {
            this.verboseLogging = verboseLogging;
        }

        public void Debug(string message)
        {
            if (verboseLogging)
            {
                DateTime now = DateTime.Now;
                string name = Thread.CurrentThread.Name;

                Console.WriteLine($"DEBUG | {now} | {name} | " + message);
            }
        }

        public void Info(string message)
        {
            DateTime now = DateTime.Now;
            string name = Thread.CurrentThread.Name;

            Console.WriteLine($"INFO | {now} | {name} | " + message);
        }
    }
}
