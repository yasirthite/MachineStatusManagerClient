using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayMachineStatusManagement.Common
{
    internal static partial class Extensions
    {
        public static bool IsEmpty(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.Empty;
        }

        public static bool IsS1_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S1_ON_FWD;
        }

        public static bool IsS1_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S1_OFF_FWD;
        }

        public static bool IsS5_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_ON_FWD;
        }

        public static bool IsS5_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_OFF_FWD;
        }
    }
}
