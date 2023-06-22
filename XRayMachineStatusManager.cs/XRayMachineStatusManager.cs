﻿using System;
using System.Threading;
using System.Threading.Tasks;
using XRayMachineStatusManagement.Sensors;

namespace XRayMachineStatusManagement
{
    public class XRayMachineStatusManager
    {
        private readonly ConsoleLogger _logger;
        private readonly bool _suppressInvalidValueException;
        private bool CanTurnOffSource = false;

        private SensorS1 sensorS1;
        private SensorS2 sensorS2;
        private SensorS3 sensorS3;
        private SensorS4 sensorS4;
        private SensorS5 sensorS5;

        public event EventHandler<SensorCode> TurnOnSource;
        public event EventHandler<SensorCode> TurnOffSource;
        public event EventHandler<SensorCode> TurnOnDetector1;
        public event EventHandler<SensorCode> TurnOffDetector1;
        public event EventHandler<SensorCode> TurnOnDetector2;
        public event EventHandler<SensorCode> TurnOffDetector2;


        public XRayMachineStatusManager()
        {
            _suppressInvalidValueException = true;
            _logger = new ConsoleLogger();

            sensorS1 = SensorS1.Instance;
            sensorS1.CanStopSource += SensorS1_CanStopSourceHandler; ;

            sensorS2 = SensorS2.Instance;

            sensorS3 = SensorS3.Instance;

            sensorS4 = SensorS4.Instance;

            sensorS5 = SensorS5.Instance;
            sensorS5.CanStopSource += SensorS5_CanStopSourceHandler;
        }

        private void SensorS5_CanStopSourceHandler()
        {
            CanTurnOffSource = true;

            if (!IsBagInTunnel && IsSourceOn)
            {
                IsSourceOn = false;

                _logger.LogInformation($"[{nameof(SensorS5_CanStopSourceHandler)}]: Source is turning OFF ...");

                if (nBagsFullyInsideTunnel < 0)
                    nBagsFullyInsideTunnel = 0;

                if (nBagsPartiallyInsideTunnel < 0)
                    nBagsPartiallyInsideTunnel = 0;

                TurnOffSource?.Invoke(this, SensorCode.SourceOnCircuitBreaker);

                CanTurnOffSource = false;

                LogBagData();
            }
        }

        private void SensorS1_CanStopSourceHandler()
        {
            CanTurnOffSource = true;

            _logger.LogInformation($"[{nameof(SensorS1_CanStopSourceHandler)}]: CanTurnOffSource = True.");

            if (!IsBagInTunnel && IsSourceOn)
            {
                IsSourceOn = false;

                _logger.LogInformation($"[{nameof(SensorS1_CanStopSourceHandler)}]: Source is turning OFF ...");

                TurnOffSource?.Invoke(this, SensorCode.SourceOnCircuitBreaker);

                CanTurnOffSource = false;

                LogBagData();
            }
        }

        public void DecideStatus(SensorCode sensorCode)
        {
            try
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}][DECIDE_STATUS: -----> [{sensorCode}]");

                SensorRecord newSensorRecord = new SensorRecord() { sensorCode = sensorCode, timeStamp = DateTime.Now };

                switch (sensorCode)
                {
                    case SensorCode.S1_ON_FWD:

                        CanTurnOffSource = false;

                        if (sensorS1.HasValid(newSensorRecord))
                        {

                            if (!IsSourceOn)
                            {
                                _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{(int)sensorCode}]: Source is Turning ON ...");

                                TurnOnSource?.Invoke(this, sensorCode);

                                IsSourceOn = true;

                                LogBagData();

                                break;
                            }
                            else
                            {
                                // _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{((int)sensorCode)}]: SKIP -> Source Already ON.");
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                            break;
                        }

                        break;

                    case SensorCode.S1_OFF_FWD:

                        if (sensorS1.HasValid(newSensorRecord))
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S2_ON_FWD:

                        if (sensorS2.HasValid(newSensorRecord))
                        {
                            nS2BagsPartial++;

                            if (!IsDetector1_On)
                            {
                                IsDetector1_On = true;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning ON ...");
                                TurnOnDetector1?.Invoke(this, sensorCode);
                                LogBagData();
                            }
                            else
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Detector 1 is already ON.");
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S2_OFF_FWD:

                        if (sensorS2.HasValid(newSensorRecord))
                        {
                            nS2BagsPartial--;
                            nS2BagsFull++;

                            _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Updated bag's data.");

                            LogBagData();
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S3_ON_FWD:

                        if (sensorS3.HasValid(newSensorRecord))
                        {
                            {
                                nS3BagsPartial++;

                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Updated bag's data.");
                                
                                LogBagData();
                            }

                            if (!IsDetector2_On)
                            {
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning ON ...");
                                
                                TurnOnDetector2?.Invoke(this, sensorCode);

                                IsDetector2_On = true;

                                LogBagData();
                            }
                            else
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Detector 2 is already ON.");
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S3_OFF_FWD:

                        if (sensorS3.HasValid(newSensorRecord))
                        {
                            {
                                nS2BagsFull--;

                                nS3BagsPartial--;
                                nS3BagsFull++;

                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Updated bag's data.");

                                LogBagData();
                            }

                            if (CanTurnOffDetector1)
                            {
                                if (IsDetector1_On)
                                {
                                    IsDetector1_On = false;
                                    _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning OFF ...");
                                    TurnOffDetector1?.Invoke(this, sensorCode);

                                    LogBagData();
                                }
                                else
                                {
                                    //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Detector 1 is already OFF.");
                                }
                            }
                            else
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> New Bag is ready for Detector 1. Hence not turning it OFF.");
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S4_ON_FWD:

                        if (sensorS4.HasValid(newSensorRecord))
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Un-used Sensor Signal.");
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S4_OFF_FWD:
                        if (sensorS4.HasValid(newSensorRecord))
                        {
                            {
                                nS3BagsFull--;

                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Updated bag's data.");
                                LogBagData();
                            }

                            if (CanTurnOffDetector2)
                            {
                                if (IsDetector2_On)
                                {
                                    IsDetector2_On = false;

                                    _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning OFF ...");
                                    TurnOffDetector2?.Invoke(this, sensorCode);

                                    LogBagData();
                                }
                                else
                                {
                                    //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Detector 2 is already OFF.");
                                }
                            }
                            else
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> New Bag is ready for Detector 2. Hence NOT turning it OFF.");
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;

                    case SensorCode.S5_ON_FWD:
                        {
                            if (sensorS5.HasValid(newSensorRecord))
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
                            }
                            else
                            {
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                            }

                            break;
                        }

                    case SensorCode.S5_OFF_FWD:

                        if (sensorS5.HasValid(newSensorRecord))
                        {
                            if (!IsAnyDetectorOn)
                            {
                                if (CanTurnOffSource)
                                {
                                    if (IsSourceOn)
                                    {
                                        IsSourceOn = false;

                                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Source is turning OFF ...");

                                        TurnOffSource?.Invoke(this, sensorCode);

                                        CanTurnOffSource = false;

                                        LogBagData();

                                    }
                                    else
                                    {
                                        //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Indicates Source is already OFF.");
                                        //LogBagData();


                                    }
                                }
                                else
                                {
                                    /////////Indicates Bag has just entered the tunnel for processing. 
                                    //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Indicates Bag has just entered the tunnel and not reached S2_ON");
                                }
                            }
                            else
                            {
                                ///////indicates detection is ON.
                                //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector(s) are ON.");
                                //LogBagData();
                            }
                        }
                        else
                        {
                            //_logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Invalid Sensor Signal Sequence");
                        }

                        break;


                    default:
                        if (_suppressInvalidValueException)
                        {
                            _logger.LogInformation($"[{typeof(SensorCode)}:{typeof(XRayMachineStatusManager)}]: Invalid value: {sensorCode}");
                        }
                        else
                        {
                            throw new ArgumentException($"[{typeof(SensorCode)}:{typeof(XRayMachineStatusManager)}]: Invalid value: {sensorCode}");
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing status: {ex.Message}");
            }
        }

        private void LogBagData()
        {
            _logger.LogInformation($"Total Bags In For Detector 1: {nS2BagsFull + nS2BagsPartial}");
            _logger.LogInformation($"Total Bags In For Detector 2: {nS3BagsFull + nS3BagsPartial}");
            _logger.LogInformation($"CanTurnOffSource: {CanTurnOffSource}");
            _logger.LogInformation($"IsSourceON: {IsSourceOn}");
            _logger.LogInformation($"IsDetector1 ON: {IsDetector1_On}");
            _logger.LogInformation($"IsDetector2 ON: {IsDetector2_On}");
        }

        private int nS2BagsPartial = 0;
        private int nS2BagsFull = 0;
        private int nS3BagsPartial = 0;
        private int nS3BagsFull = 0;

        private bool CanTurnOffDetector1 => !(nS2BagsPartial > 0 || nS2BagsFull > 0);
        private bool CanTurnOffDetector2 => !(nS3BagsPartial > 0 || nS3BagsFull > 0);
        private bool IsAnyDetectorOn => (IsDetector1_On || IsDetector2_On);

        private bool IsSourceOn = false;
        private bool IsDetector1_On = false;
        private bool IsDetector2_On = false;
    }
}
