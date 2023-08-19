// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------


using System;
using System.Threading;
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

        public SensorRecord SensorRecord { get { return _prevSensorRecord; } }
        private SensorRecord _prevSensorRecord;
        private IMachineStatusLogger machineStatusLogger = default;
        private const int SensorWaitTimeInMilliseconds = 0;
        private bool IsSourceONCircuitBreakerPutToTrigger = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Sensor's Wait Time Window in milliseconds.
        /// </summary>
        private TimeSpan SensorWaitTimeWindow = TimeSpan.FromMilliseconds(SensorWaitTimeInMilliseconds);

        /// <summary>
        /// This time window causes Source-ON-Circuit to break by firing event CanStopSource.
        /// </summary>
        private TimeSpan SourceStopTimeWindow = TimeSpan.FromMilliseconds(4000);

        private SensorS1(IMachineStatusLogger logger)
        {
            this.machineStatusLogger = logger;
            _prevSensorRecord = new SensorRecord() { sensorCode = SensorCode.Empty, timeStamp = DateTime.MinValue };
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
            _prevSensorRecord = newSensorRecord;

            if (newSensorRecord.sensorCode.IsS1_ON_FWD())
            {
                TriggerCanStopSourceAfterSourceStopTimeWindowIsComplete();

                //if (!IsSourceONCircuitBreakerPutToTrigger)
                //{
                //    IsSourceONCircuitBreakerPutToTrigger = true;

                //    cancellationTokenSource = new CancellationTokenSource();

                //    Task.Delay(SourceStopTimeWindow, cancellationTokenSource.Token)
                //        .ContinueWith(_ => CanStopSource?.Invoke())
                //        .ContinueWith(_ => IsSourceONCircuitBreakerPutToTrigger = false);
                //}
            }

            return true;
        }

        private void TriggerCanStopSourceAfterSourceStopTimeWindowIsComplete()
        {
            // Capture the previous token before canceling it
            //var previousToken = cancellationTokenSource.Token;

            // Cancel the previous task
            cancellationTokenSource.Cancel();

            // Create a new CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();

            // Use the previous token to prevent the race condition
            Task.Delay(SourceStopTimeWindow, cancellationTokenSource.Token)
                .ContinueWith(_ => CanStopSource?.Invoke(), TaskContinuationOptions.NotOnCanceled);
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
            return ((_prevSensorRecord.sensorCode.IsS1_ON_FWD() && newSensorRecord.sensorCode.IsS1_ON_FWD()) ||
                            ((_prevSensorRecord.sensorCode.IsS1_OFF_FWD() || _prevSensorRecord.sensorCode.IsEmpty()) && newSensorRecord.sensorCode.IsS1_OFF_FWD()));
        }
    }
}
