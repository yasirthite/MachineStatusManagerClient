// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using XRayMachineStatusManagement.Common;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal sealed class SensorS1
    {
        public event Action CanStopSource;

        public static SensorS1 Instance
        {
            get
            {
                return lazySensor.Value;
            }
        }

        private SensorRecord Prev_SensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;
        private const int SensorWaitTimeInMilliseconds = 0;
        private bool IsSourceONCircuitBreakerPutToTrigger = false;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(SensorWaitTimeInMilliseconds);

        /// <summary>
        /// This time window causes Source-ON-Circuit to break by firing event CanStopSource.
        /// </summary>
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(2500);

        private SensorS1(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            Prev_SensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS1> lazySensor = new Lazy<SensorS1>(() => new SensorS1(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            if (newSensorRecord.sensorCode.IsS1_ON_FWD() || newSensorRecord.sensorCode.IsS1_OFF_FWD())
                return NewCheckValidityForForwardDirection(newSensorRecord);
            else
                return CheckValidityForReverseDirection(newSensorRecord);
        }

        private bool CheckValidityForReverseDirection(SensorRecord newSensorRecord)
        {
            return true;
        }

        private bool NewCheckValidityForForwardDirection(SensorRecord newSensorRecord)
        {
            Prev_SensorRecord = newSensorRecord;

            if (newSensorRecord.sensorCode.IsS1_ON_FWD())
            {
                if (!IsSourceONCircuitBreakerPutToTrigger)
                {
                    IsSourceONCircuitBreakerPutToTrigger = true;

                    Task.Delay(SourceStopTimeWindow)
                        .ContinueWith(_ => CanStopSource?.Invoke())
                        .ContinueWith(_ => IsSourceONCircuitBreakerPutToTrigger = false);
                }
            }

            return true;
        }

        private bool IsProhibitedTimeWindowOpenFor(SensorRecord newSensorRecord)
        {
            if (SensorWaitTimeInMilliseconds <= 0)
                return false;

            TimeSpan timeStampDifference = newSensorRecord.timeStamp - Prev_SensorRecord.timeStamp;
            return timeStampDifference < SensorWaitTimeWindow;
        }

        private bool PrevSensorRecordHasInvalidSequenceWith(SensorRecord newSensorRecord)
        {
            return ((Prev_SensorRecord.sensorCode.IsS1_ON_FWD() && newSensorRecord.sensorCode.IsS1_ON_FWD()) ||
                            ((Prev_SensorRecord.sensorCode.IsS1_OFF_FWD() || Prev_SensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS1_OFF_FWD()));
        }
    }
}
