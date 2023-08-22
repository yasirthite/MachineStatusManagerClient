// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------

using System;
using XRayMachineStatusManagement.Common;
using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManagement.Sensors
{
    internal sealed class SensorS2
    {
        public event Action FaultySensorData;
        public static SensorS2 Instance
        {
            get
            {
                return lazySensor.Value;
            }
        }

        public SensorRecord SensorRecord { get { return _prevSensorRecord; } }
        private SensorRecord _prevSensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;
        public const int SensorBlinkTimeInMilliseconds = 250;
        private int BagNumber = 0;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorProhibitedTimeWindow = TimeSpan.FromMilliseconds(SensorBlinkTimeInMilliseconds);

        private SensorS2(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            _prevSensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS2> lazySensor = new Lazy<SensorS2>(() => new SensorS2(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            if (newSensorRecord.sensorCode.IsS2_ON_FWD() || newSensorRecord.sensorCode.IsS2_OFF_FWD())
                return CheckValidityForForwardDirection(newSensorRecord);
            else
                return CheckValidityForReverseDirection(newSensorRecord);
        }

        private bool CheckValidityForReverseDirection(SensorRecord newSensorRecord)
        {
            return CheckValidityForForwardDirection(newSensorRecord);
        }

        private bool CheckValidityForForwardDirection(SensorRecord newSensorRecord)
        {
            if (HasValidSequence(newSensorRecord))
            {
                if (IsTurningOFF(newSensorRecord))
                {
                    if (IsProhibitedTimeWindowOpenFor(newSensorRecord))
                    {
                        //Todo: decrement nS2PartiallBags due to previous faulty sensor data. Use event for time being.
                        FaultySensorData?.Invoke();
                        _prevSensorRecord = newSensorRecord;
                        return false;
                    }
                    else
                    {
                        //Indicates: Prohibited Window is NOT open. You can safely take the value.
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($">>------------------------------------------->>[WESI][BAG Number: {++BagNumber}] " +
                            $"Time to pass through S2({newSensorRecord.sensorCode} - {_prevSensorRecord.sensorCode}) " +
                            $"= {(newSensorRecord.timeStamp - _prevSensorRecord.timeStamp).TotalMilliseconds} ms.");
                        Console.ResetColor();

                        _prevSensorRecord = newSensorRecord;
                        return true;
                    }
                }
                else
                {
                    //Indicates: Sensor is Turning ON.
                    _prevSensorRecord = newSensorRecord;
                    return true;
                }
            }
            else
            {
                //Indicates: Sensor Data Sequence is Invalid. i:e, Receiving two consecutive ONs or OFFs.
                _prevSensorRecord.timeStamp = newSensorRecord.timeStamp;
                return false;
            }
        }

        private bool IsTurningOFF(SensorRecord newSensorRecord)
        {
            return _prevSensorRecord.sensorCode.IsS2_ON_FWD() && newSensorRecord.sensorCode.IsS2_OFF_FWD() ||
                _prevSensorRecord.sensorCode.IsS2_ON_REV() && newSensorRecord.sensorCode.IsS2_OFF_REV();
        }

        private bool IsProhibitedTimeWindowOpenFor(SensorRecord newSensorRecord)
        {
            TimeSpan timeStampDifference = newSensorRecord.timeStamp - _prevSensorRecord.timeStamp;
            return timeStampDifference < SensorProhibitedTimeWindow;
        }

        private bool HasValidSequence(SensorRecord newSensorRecord)
        {
            return ! ((_prevSensorRecord.sensorCode.IsS2_ON_FWD() && newSensorRecord.sensorCode.IsS2_ON_FWD()) ||
                (_prevSensorRecord.sensorCode.IsS2_ON_REV() && newSensorRecord.sensorCode.IsS2_ON_REV()) ||
                ((_prevSensorRecord.sensorCode.IsS2_OFF_FWD() || _prevSensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS2_OFF_FWD()) ||
                (_prevSensorRecord.sensorCode.IsS2_OFF_REV() || _prevSensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS2_OFF_REV());
        }
    }
}