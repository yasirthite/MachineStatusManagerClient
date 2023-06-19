﻿using System;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal class SensorS2
    {
        private SensorRecord Prev_SensorRecord = default;
        IMachineStatusLogger machineStatusLogger = default;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(500);

        internal SensorS2(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
        }
    }
}
