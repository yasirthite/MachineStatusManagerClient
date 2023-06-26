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

            manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(94).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
        }

        private void Manager_TurnOffDetector2(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn OFF Detector 2.");
        }

        private void Manager_TurnOnDetector2(object sender, SensorCode e)
        {

            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn ON Detector 2.");
        }

        private void Manager_TurnOffDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn OFF Detector 1.");
        }

        private void Manager_TurnOnDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn ON Detector 1.");
        }

        private void Manager_TurnOffSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn OFF Source.");
        }

        private void Manager_TurnOnSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]" +
                $"[{DateTime.Now.TimeOfDay.TotalMilliseconds}]" +
                $"[{e}:{(int)e}]: Event Handled => Turn ON Source.");
        }
    }
}
