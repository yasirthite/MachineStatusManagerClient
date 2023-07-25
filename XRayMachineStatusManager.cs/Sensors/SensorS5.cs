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

        public SensorRecord SensorRecord { get { return _prevSensorRecord; } }
        private SensorRecord _prevSensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;
        private const int SensorWaitTimeInMilliseconds = 0;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(SensorWaitTimeInMilliseconds);
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(4000);

        private SensorS5(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            _prevSensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS5> lazySensor = new Lazy<SensorS5>(() => new SensorS5(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            return true;

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
            if (_prevSensorRecord.sensorCode.IsEmpty())
            {
                _prevSensorRecord = newSensorRecord;
                Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
                return true;
            }
            else if (IsProhibitedTimeWindowOpenFor(newSensorRecord))
            {
                if (_prevSensorRecord.sensorCode.IsS5_ON_FWD() && newSensorRecord.sensorCode.IsS5_OFF_FWD())
                    _prevSensorRecord = newSensorRecord;
                else
                    _prevSensorRecord.timeStamp = newSensorRecord.timeStamp;

                return false;
            }
            else if (PrevSensorRecordHasInvalidSequenceWith(newSensorRecord))
            {
                _prevSensorRecord.timeStamp = newSensorRecord.timeStamp;
                return false;
            }
            else
            {
                _prevSensorRecord = newSensorRecord;
                Task.Delay(SourceStopTimeWindow).ContinueWith(_ => CanStopSource?.Invoke());
                return true;
            }
        }

        private bool IsProhibitedTimeWindowOpenFor(SensorRecord newSensorRecord)
        {
            if (SensorWaitTimeInMilliseconds <= 0)
                return false;

            TimeSpan timeStampDifference = newSensorRecord.timeStamp - _prevSensorRecord.timeStamp;
            return timeStampDifference < SensorWaitTimeWindow;
        }

        private bool PrevSensorRecordHasInvalidSequenceWith(SensorRecord newSensorRecord)
        {
            return ((_prevSensorRecord.sensorCode.IsS5_ON_FWD() && newSensorRecord.sensorCode.IsS5_ON_FWD()) ||
                            ((_prevSensorRecord.sensorCode.IsS5_OFF_FWD() || _prevSensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS5_OFF_FWD()));
        }
    }
}
