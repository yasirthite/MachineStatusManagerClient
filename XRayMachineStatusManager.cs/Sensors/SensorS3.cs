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
    internal sealed class SensorS3
    {
        public event Action FaultySensorData;
        public static SensorS3 Instance
        {
            get
            {
                return lazySensor.Value;
            }
        }

        private SensorRecord Prev_SensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;
        public const int SensorProhibitedtimeInMilliseconds = 200;
        private int BagNumber = 0;

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorProhibitedTimeWindow = TimeSpan.FromMilliseconds(SensorProhibitedtimeInMilliseconds);

        /// <summary>
        /// This time window causes Source-ON-Circuit to break by firing event CanStopSource.
        /// </summary>
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(4000);

        private SensorS3(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            Prev_SensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
        }

        private static readonly Lazy<SensorS3> lazySensor = new Lazy<SensorS3>(() => new SensorS3(new ConsoleLogger()));

        internal bool HasValid(SensorRecord newSensorRecord)
        {
            if (newSensorRecord.sensorCode.IsS3_ON_FWD() || newSensorRecord.sensorCode.IsS3_OFF_FWD())
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
            if (HasValidSequence(newSensorRecord))
            {
                if (IsTurningOFF(newSensorRecord))
                {
                    if (IsProhibitedTimeWindowOpenFor(newSensorRecord))
                    {
                        //Todo: decrement nS2PartiallBags due to previous faulty sensor data. Use event for time being.
                        FaultySensorData?.Invoke();
                        Prev_SensorRecord = newSensorRecord;
                        return false;
                    }
                    else
                    {
                        //Indicates: Prohibited Window is NOT open. You can safely take the value.
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($">>------------------------------------------->>[WESI][BAG Number: {++BagNumber}] " +
                            $"Time to pass through S3({newSensorRecord.sensorCode} - {Prev_SensorRecord.sensorCode}) " +
                            $"= {(newSensorRecord.timeStamp - Prev_SensorRecord.timeStamp).TotalMilliseconds} ms.");
                        Console.ResetColor();

                        Prev_SensorRecord = newSensorRecord;
                        return true;
                    }
                }
                else
                {
                    //Indicates: Sensor is Turning ON.
                    Prev_SensorRecord = newSensorRecord;
                    return true;
                }
            }
            else
            {
                //Indicates: Sensor Data Sequence is Invalid. i:e, Receiving two consecutive ONs or OFFs.
                Prev_SensorRecord.timeStamp = newSensorRecord.timeStamp;
                return false;
            }
        }

        private bool IsTurningOFF(SensorRecord newSensorRecord)
        {
            return Prev_SensorRecord.sensorCode.IsS3_ON_FWD() && newSensorRecord.sensorCode.IsS3_OFF_FWD();
        }

        private bool IsProhibitedTimeWindowOpenFor(SensorRecord newSensorRecord)
        {
            TimeSpan timeStampDifference = newSensorRecord.timeStamp - Prev_SensorRecord.timeStamp;
            return timeStampDifference < SensorProhibitedTimeWindow;
        }

        private bool HasValidSequence(SensorRecord newSensorRecord)
        {
            return !((Prev_SensorRecord.sensorCode.IsS3_ON_FWD() && newSensorRecord.sensorCode.IsS3_ON_FWD()) ||
                            ((Prev_SensorRecord.sensorCode.IsS3_OFF_FWD() || Prev_SensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS3_OFF_FWD()));
        }
    }
}

