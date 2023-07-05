// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------


using System;
using System.Threading;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement
{
    internal class ConsoleLogger: IMachineStatusLogger
    {
        public static string GetMessageHeader => $"[WESI][{Thread.CurrentThread.ManagedThreadId}][{DateTime.Now}][{DateTime.Now.TimeOfDay.TotalMilliseconds}]";

        public ConsoleLogger() 
        {
        }

        public void LogInformation(string message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(GetMessageHeader + message);
                Console.ResetColor();
            }
        }

        public void LogError(string message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(GetMessageHeader + message);
                Console.ResetColor();
            }
        }

        public void LogWarning(string message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(GetMessageHeader + message);
                Console.ResetColor();
            }
        }

        public void LogCritical(string message)
        {
            //lock (this)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(GetMessageHeader + message);
                Console.ResetColor();
            }
        }
    }
}
