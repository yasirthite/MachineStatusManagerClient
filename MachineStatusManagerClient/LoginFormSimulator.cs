using System;
using System.Threading;
using System.Threading.Tasks;
using XRayMachineStatusManagement;

namespace MachineStatusManagerClient
{
    internal class LoginFormSimulator
    {
        XRayMachineStatusManagement.XRayMachineStatusManager manager;

        public LoginFormSimulator()
        {
            manager = new XRayMachineStatusManagement.XRayMachineStatusManager();

            manager.TurnOnSource += Manager_TurnOnSource;
            manager.TurnOffSource += Manager_TurnOffSource;
            manager.TurnOnDetector1 += Manager_TurnOnDetector1;
            manager.TurnOffDetector1 += Manager_TurnOffDetector1;
            manager.TurnOnDetector2 += Manager_TurnOnDetector2;
            manager.TurnOffDetector2 += Manager_TurnOffDetector2;
        }

        public void StartSimulation()
        {
            Console.WriteLine($"[LOGIN_FORM_START:-----> {Thread.CurrentThread.ManagedThreadId}]");

            SimulationData.Execute(this.manager);

            //manager.DecideStatus(SensorCode.S4_ON_FWD);
            //Task.Delay(94).Wait(); manager.DecideStatus(SensorCode.S4_OFF_FWD);
            //Task.Delay(194).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
        }

        private void Manager_TurnOnDetector2(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnDetector2)}] EventHandled -->> Detector 2 is On");
        }

        private void Manager_TurnOffDetector2(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffDetector2)}] EventHandled -->> Detector 2 is Off");
        }

        private void Manager_TurnOffDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffDetector1)}] EventHandled -->> Detector 1 is Off");
        }

        private void Manager_TurnOnDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnDetector1)}] EventHandled -->> Detector 1 is On");
        }

        private void Manager_TurnOffSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffSource)}] EventHandled -->> Source is Off");
        }

        private void Manager_TurnOnSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnSource)}] EventHandled -->> Source is On");
        }
    }
}
