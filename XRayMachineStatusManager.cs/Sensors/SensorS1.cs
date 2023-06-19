using System;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal class SensorS1
    {
        private SensorRecord Prev_SensorRecord = default;
        IMachineStatusLogger machineStatusLogger = default;
        internal bool MyProperty { get; } 

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(500);
        
        internal SensorS1(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
        }

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            TimeSpan timeStampDifference = newSensorRecord.timeStamp - Prev_SensorRecord.timeStamp;

            if (Prev_SensorRecord.timeStamp == DateTime.MinValue)
            {
                machineStatusLogger.LogInformation($"Prev_SensorRecord_S1 {Prev_SensorRecord.timeStamp.Millisecond} ms ===============> 0 (VALID)");

                Prev_SensorRecord = newSensorRecord;

                return true;
            }
            else if (timeStampDifference > SensorWaitTimeWindow)
            {
                machineStatusLogger.LogInformation($"Difference in prev and current SensorRecord_S1 => " +
                    $"{timeStampDifference.TotalMilliseconds} ms ================> (IN-VALID)");

                Prev_SensorRecord.timeStamp = newSensorRecord.timeStamp;

                return false;
            }
            else
            {
                machineStatusLogger.LogInformation($"{timeStampDifference.TotalMilliseconds} ms ================> (VALID)");

                Prev_SensorRecord = newSensorRecord;

                return true;
            }
        }
    }
}
