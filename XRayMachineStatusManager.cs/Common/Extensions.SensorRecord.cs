// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------


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
        public static bool IsS1_ON_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S1_ON_REV;
        }


        public static bool IsS1_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S1_OFF_FWD;
        }
        public static bool IsS1_OFF_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S1_ON_REV;
        }


        public static bool IsS2_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_ON_FWD;
        }
        public static bool IsS2_ON_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_ON_REV;
        }

        public static bool IsS2_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_OFF_FWD;
        }
        public static bool IsS2_OFF_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S2_OFF_REV;
        }

        public static bool IsS3_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_ON_FWD;
        }
        public static bool IsS3_ON_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_ON_REV;
        }

        public static bool IsS3_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_OFF_FWD;
        }
        public static bool IsS3_OFF_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S3_OFF_REV;
        }

        public static bool IsS4_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_ON_FWD;
        }
        public static bool IsS4_ON_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_ON_REV;
        }

        public static bool IsS4_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_OFF_FWD;
        }
        public static bool IsS4_OFF_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S4_OFF_REV;
        }

        public static bool IsS5_ON_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_ON_FWD;
        }
        public static bool IsS5_ON_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_ON_REV;
        }

        public static bool IsS5_OFF_FWD(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_OFF_FWD;
        }
        public static bool IsS5_OFF_REV(this SensorCode sensorCode)
        {
            return sensorCode == SensorCode.S5_OFF_REV;
        }
    }
}
