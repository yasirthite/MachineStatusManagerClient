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

            manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(78).Wait(); manager.DecideStatus(SensorCode.S1_OFF_FWD);
            Task.Delay(78).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(1343).Wait(); manager.DecideStatus(SensorCode.S2_ON_FWD);
            Task.Delay(921).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
            Task.Delay(937).Wait(); manager.DecideStatus(SensorCode.S3_OFF_FWD);
            Task.Delay(937).Wait(); manager.DecideStatus(SensorCode.S4_ON_FWD);
            Task.Delay(937).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(187).Wait(); manager.DecideStatus(SensorCode.S2_ON_FWD);
            Task.Delay(749).Wait(); manager.DecideStatus(SensorCode.S4_OFF_FWD);
            Task.Delay(156).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(46).Wait(); manager.DecideStatus(SensorCode.S2_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
            Task.Delay(46).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(46).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(896).Wait(); manager.DecideStatus(SensorCode.S4_ON_FWD);
            Task.Delay(896).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(62).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(156).Wait(); manager.DecideStatus(SensorCode.S1_OFF_FWD);
            Task.Delay(156).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S2_ON_FWD);
            Task.Delay(0).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(109).Wait(); manager.DecideStatus(SensorCode.S1_OFF_FWD);
            Task.Delay(187).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S1_OFF_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(406).Wait(); manager.DecideStatus(SensorCode.S3_OFF_FWD);
            Task.Delay(140).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(140).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(46).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S3_OFF_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
            Task.Delay(93).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(374).Wait(); manager.DecideStatus(SensorCode.S1_ON_FWD);
            Task.Delay(374).Wait(); manager.DecideStatus(SensorCode.S1_OFF_FWD);
            Task.Delay(171).Wait(); manager.DecideStatus(SensorCode.S2_ON_FWD);
            Task.Delay(109).Wait(); manager.DecideStatus(SensorCode.S4_OFF_FWD);
            Task.Delay(156).Wait(); manager.DecideStatus(SensorCode.S4_ON_FWD);
            Task.Delay(140).Wait(); manager.DecideStatus(SensorCode.S3_OFF_FWD);
            Task.Delay(471).Wait(); manager.DecideStatus(SensorCode.S2_OFF_FWD);
            Task.Delay(78).Wait(); manager.DecideStatus(SensorCode.S3_ON_FWD);
            Task.Delay(374).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(35).Wait(); manager.DecideStatus(SensorCode.S4_OFF_FWD);
            Task.Delay(81).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(381).Wait(); manager.DecideStatus(SensorCode.S3_OFF_FWD);
            Task.Delay(93).Wait(); manager.DecideStatus(SensorCode.S4_ON_FWD);
            Task.Delay(1520).Wait(); manager.DecideStatus(SensorCode.S4_OFF_FWD);
            Task.Delay(1218).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(437).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(140).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(15).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(31).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(218).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(46).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);
            Task.Delay(81).Wait(); manager.DecideStatus(SensorCode.S5_ON_FWD);
            Task.Delay(37).Wait(); manager.DecideStatus(SensorCode.S5_OFF_FWD);


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
