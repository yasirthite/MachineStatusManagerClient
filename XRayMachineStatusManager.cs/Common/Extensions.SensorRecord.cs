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

        public static bool IsS2_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_ON_FWD;
        }

        public static bool IsS2_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_OFF_FWD;
        }

        public static bool IsS3_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_ON_FWD;
        }

        public static bool IsS3_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_OFF_FWD;
        }

        public static bool IsS4_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_ON_FWD;
        }

        public static bool IsS4_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_OFF_FWD;
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
