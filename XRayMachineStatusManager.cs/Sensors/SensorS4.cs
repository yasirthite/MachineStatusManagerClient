using System;
using XRayMachineStatusManagement.Common;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal sealed class SensorS4
    {
        public static SensorS4 Instance
        {
            get
            {
                return lazySensor.Value;
            }
        }

        private SensorRecord Prev_SensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(0);

        /// <summary>
        /// This time window causes Source-ON-Circuit to break by firing event CanStopSource.
        /// </summary>
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(4000);

        private SensorS4(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            Prev_SensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS4> lazySensor = new Lazy<SensorS4>(() => new SensorS4(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            if (newSensorRecord.sensorCode.IsS4_ON_FWD() || newSensorRecord.sensorCode.IsS4_OFF_FWD())
                return CheckValidityForForwardDirection(newSensorRecord);
            else
                return CheckValidityForReverseDirection(newSensorRecord);
        }

        private bool CheckValidityForReverseDirection(SensorRecord newSensorRecord)
        {
            return true;
        }

        private bool CheckValidityForForwardDirection(SensorRecord newSensorRecord)
        {
            if (Prev_SensorRecord.sensorCode.IsEmpty())
            {
                Prev_SensorRecord = newSensorRecord;
                //Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
                return true;
            }
            else if (IsProhibitedTimeWindowOpenFor(newSensorRecord))
            {
                if (Prev_SensorRecord.sensorCode.IsS4_ON_FWD() && newSensorRecord.sensorCode.IsS4_OFF_FWD())
                    Prev_SensorRecord = newSensorRecord;
                else
                    Prev_SensorRecord.timeStamp = newSensorRecord.timeStamp;

                return false;
            }
            else if (PrevSensorRecordHasInvalidSequenceWith(newSensorRecord))
            {
                Prev_SensorRecord.timeStamp = newSensorRecord.timeStamp;
                return false;
            }
            else
            {
                Prev_SensorRecord = newSensorRecord;
                //Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
                return true;
            }
        }

        private bool IsProhibitedTimeWindowOpenFor(SensorRecord newSensorRecord)
        {
            TimeSpan timeStampDifference = newSensorRecord.timeStamp - Prev_SensorRecord.timeStamp;
            return timeStampDifference < SensorWaitTimeWindow;
        }

        private bool PrevSensorRecordHasInvalidSequenceWith(SensorRecord newSensorRecord)
        {
            return (Prev_SensorRecord.sensorCode.IsS4_ON_FWD() && newSensorRecord.sensorCode.IsS4_ON_FWD()) ||
                            (Prev_SensorRecord.sensorCode.IsS4_OFF_FWD() && newSensorRecord.sensorCode.IsS4_OFF_FWD());
        }
    }
}