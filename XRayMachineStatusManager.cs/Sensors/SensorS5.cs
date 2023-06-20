using System;
using System.Threading.Tasks;
using XRayMachineStatusManagement.Common;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal sealed class SensorS5
    {
        public event Action CanStopSource;

        public static SensorS5 Instance
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
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(2000);
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(4000);

        private SensorS5(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            Prev_SensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS5> lazySensor = new Lazy<SensorS5>(() => new SensorS5(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            if (newSensorRecord.sensorCode.IsS5_ON_FWD() || newSensorRecord.sensorCode.IsS5_OFF_FWD())
                return CheckValidityForForwardDirection(newSensorRecord);
            else
                return CheckValidityForReverseDirection(newSensorRecord);
        }

        private bool CheckValidityForForwardDirection(SensorRecord newSensorRecord)
        {
            //Todo: No Check Required.
            return true;
        }

        private bool CheckValidityForReverseDirection(SensorRecord newSensorRecord)
        {
            if (Prev_SensorRecord.sensorCode.IsEmpty())
            {
                Prev_SensorRecord = newSensorRecord;
                Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
                return true;
            }
            else if (IsProhibitedTimeWindowOpenFor(newSensorRecord))
            {
                if (Prev_SensorRecord.sensorCode.IsS5_ON_FWD() && newSensorRecord.sensorCode.IsS5_OFF_FWD())
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
                Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
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
            return (Prev_SensorRecord.sensorCode.IsS5_ON_FWD() && newSensorRecord.sensorCode.IsS5_ON_FWD()) ||
                            (Prev_SensorRecord.sensorCode.IsS5_OFF_FWD() && newSensorRecord.sensorCode.IsS5_OFF_FWD());
        }
    }
}
