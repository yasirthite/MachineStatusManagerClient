using System;
using System.Threading;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement
{
    internal class ConsoleLogger: IMachineStatusLogger
    {
        public ConsoleLogger() 
        {
        }

        public void LogInformation(string message)
        {
            lock (this) 
            {
                
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}][{DateTime.Now.TimeOfDay.TotalMilliseconds}]" + message);
                //Console.ResetColor();
            }
        }

        public void LogError(string message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]" + message);
                Console.ResetColor();
            }
        }

        public void LogWarning(string message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]" + message);
                Console.ResetColor();
            }
        }

        public void LogCritical(string message)
        {
            //lock (this)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]" + message);
                Console.ResetColor();
            }
        }
    }
}
