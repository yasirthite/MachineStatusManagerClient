using System;

namespace MachineStatusManagerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XRayMachineStatusManagement.XRayMachineStatusManager manager = new XRayMachineStatusManagement.XRayMachineStatusManager();
            manager.TurnOnSource += Manager_TurnOnSource;
            manager.TurnOffSource += Manager_TurnOffSource;
            manager.DecideStatusAsync(4849).Wait();
            Console.ReadLine();
        }

        private static void Manager_TurnOffSource(object sender, int e)
        {
            Console.WriteLine("Source Turned Off");
        }

        private static void Manager_TurnOnSource(object sender, int e)
        {
            Console.WriteLine("Source Turned On");
        }
    }
}
