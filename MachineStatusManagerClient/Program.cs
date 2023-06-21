using System;
using System.Threading;
using XRayMachineStatusManagement;

namespace MachineStatusManagerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[MAIN:-----> {Thread.CurrentThread.ManagedThreadId}]");
            new LoginFormSimulator().Start();
            //new LoginFormSimulator(true).StartAsync();
            Console.WriteLine($"[MAIN:-----> {Thread.CurrentThread.ManagedThreadId}]");

            Console.ReadKey();
        }
    }
}
