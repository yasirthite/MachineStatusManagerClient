using System;
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

            sensorS2 = new SensorS2(_logger);
            sensorS3 = new SensorS3(_logger);
            sensorS4 = new SensorS4(_logger);

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

                TurnOffSource?.Invoke(this, SensorCode.SourceContinuousOnCircuitBreaker);

                LogBagData();
            }
        }

        private void SensorS1_CanStopSourceHandler()
        {
            CanTurnOffSource = true;

            if(!IsBagInTunnel && IsSourceOn)
            {
                IsSourceOn = false;

                _logger.LogInformation($"[{nameof(SensorS1_CanStopSourceHandler)}]: Source is turning OFF ...");

                TurnOffSource?.Invoke(this, SensorCode.SourceContinuousOnCircuitBreaker);

                LogBagData();
            }
        }

        public async Task DecideStatusAsync(SensorCode sensorCode)
        {
            try
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}][DECIDE_STATUS_ASYNC: -----> [{sensorCode}]");

                SensorRecord newSensorRecord = new SensorRecord() { sensorCode = sensorCode, timeStamp = DateTime.Now };

                switch (sensorCode)
                {
                    case SensorCode.S1_ON_FWD:
                        {
                            if (!sensorS1.HasValid(newSensorRecord))
                                return;

                            if (!IsSourceOn)
                            {
                                IsSourceOn = true;

                                _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{(int)sensorCode}]: Source is Turning ON ...");

                                TurnOnSource?.Invoke(this, sensorCode);

                                break;
                            }
                            else
                            {
                                _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{((int)sensorCode)}]: SKIP -> Source Already ON.");
                                break;
                            }
                        }

                    case SensorCode.S1_OFF_FWD:
                        {
                            if (!sensorS1.HasValid(newSensorRecord))
                                return;

                            if (!IsSourceOn)
                            {
                                _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{(int)sensorCode}]: Source Already OFF.");

                                break;
                            }
                            else
                            {
                                CanTurnOffSource = true;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: This State is Hardly Possible. Can Safely Turn Off Source Now at S5_OFF.");

                                break;
                            }
                        }

                    case SensorCode.S2_ON_FWD:
                        {
                            nBagsPartiallyInsideTunnel++;

                            if (IsDetector1_On)
                            {
                                nBagsPartiallyInsideTunnel++;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is already ON.");
                            }
                            else
                            {
                                IsDetector1_On = true;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning ON ...");
                                TurnOnDetector1?.Invoke(this, sensorCode);
                            }
                            break;
                        }

                    case SensorCode.S2_OFF_FWD:
                        //Todo: Add fully/partially inside for detection.
                        nBagsPartiallyInsideTunnel--;
                        nBagsFullyInsideTunnel++;

                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Noted.");
                        break;

                    case SensorCode.S3_ON_FWD:
                        if (IsDetector2_On)
                        {
                            _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is already ON.");
                        }
                        else
                        {
                            IsDetector2_On = true;
                            _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning ON ...");
                            TurnOnDetector2?.Invoke(this, sensorCode);
                        }
                        break;

                    case SensorCode.S3_OFF_FWD:
                        //Todo: Add fully/partially inside for detection.
                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
                        break;

                    case SensorCode.S4_ON_FWD:
                        //Todo: Add fully/partially inside for detection.
                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
                        break;

                    case SensorCode.S4_OFF_FWD:
                        {
                            nBagsFullyInsideTunnel--;

                            if (IsDetector1_On)
                            {
                                IsDetector1_On = false;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning OFF ...");
                                TurnOffDetector1?.Invoke(this, sensorCode);
                            }
                            if (IsDetector2_On)
                            {
                                IsDetector2_On = false;
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning OFF ...");
                                TurnOffDetector2?.Invoke(this, sensorCode);
                            }

                            break;
                        }

                    case SensorCode.S5_ON_FWD:
                        {
                            _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
                            break;
                        }

                    case SensorCode.S5_OFF_FWD:
                        {
                            if (IsBagInTunnel)
                                return;

                            if (CanTurnOffSource)
                            {
                                IsSourceOn = false;

                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Source is turning OFF ...");

                                TurnOffSource?.Invoke(this, sensorCode);

                                LogBagData();
                                
                                break;
                            }
                            else
                            {
                                //Condition: (!IsBagInTunnel && !CanTurnOffSource) Indicates Bag has just entered the tunnel for processing. 
                                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: SKIP -> Indicates Bag is in for processing.");

                                break;
                            }
                        }

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

        //public void DecideStatus(SensorCode sensorCode)
        //{
        //    try
        //    {

        //        Console.WriteLine($"[DECIDE_STATUS:-----> {Thread.CurrentThread.ManagedThreadId}]");

        //        SensorRecord newSensorRecord = new SensorRecord() { sensorCode = sensorCode, timeStamp = DateTime.Now };

        //        //if (!IsValid(newSensorRecord))
        //        //    return;

        //        switch (sensorCode)
        //        {
        //            case SensorCode.S1_ON_FWD:
        //                if (IsSourceOn)
        //                {
        //                    _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{((int)sensorCode)}]: Source Already ON.");
        //                    nBagsPartiallyInsideTunnel += 1;
        //                    LogBagData();
        //                    break;
        //                }
        //                else
        //                {
        //                    IsSourceOn = true;
        //                    nBagsPartiallyInsideTunnel += 1;
        //                    _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}][{sensorCode}:{(int)sensorCode}]:Source is Turning ON ...");
        //                    LogBagData();
        //                    TurnOnSource?.Invoke(this, sensorCode);
        //                    break;
        //                }

        //            case SensorCode.S1_OFF_FWD:
        //                {
        //                    nBagsPartiallyInsideTunnel -= 1;
        //                    nBagsFullyInsideTunnel += 1;
        //                    _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: {TotalBagsInsideTunnel} Bag(s) inside the tunnel.");
        //                    break;
        //                }

        //            case SensorCode.S2_ON_FWD:
        //                {
        //                    if (IsDetector1_On)
        //                    {
        //                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is already ON.");
        //                    }
        //                    else
        //                    {
        //                        IsDetector1_On = true;
        //                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning ON ...");
        //                        TurnOnDetector1?.Invoke(this, sensorCode);
        //                    }
        //                    break;
        //                }

        //            case SensorCode.S2_OFF_FWD:
        //                //Todo: Add fully/partially inside for detection.
        //                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Noted Un-used Sensor Signal.");
        //                break;

        //            case SensorCode.S3_ON_FWD:
        //                if (IsDetector2_On)
        //                {
        //                    _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is already ON.");
        //                }
        //                else
        //                {
        //                    IsDetector2_On = true;
        //                    _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning ON ...");
        //                    TurnOnDetector2?.Invoke(this, sensorCode);
        //                }
        //                break;

        //            case SensorCode.S3_OFF_FWD:
        //                //Todo: Add fully/partially inside for detection.
        //                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
        //                break;

        //            case SensorCode.S4_ON_FWD:
        //                //Todo: Add fully/partially inside for detection.
        //                _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Un-used Sensor Signal.");
        //                break;

        //            case SensorCode.S4_OFF_FWD:
        //                {
        //                    if (IsDetector1_On)
        //                    {
        //                        IsDetector1_On = false;
        //                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 1 is turning OFF ...");
        //                        TurnOffDetector1?.Invoke(this, sensorCode);
        //                    }
        //                    if (IsDetector2_On)
        //                    {
        //                        IsDetector2_On = false;
        //                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Detector 2 is turning OFF ...");
        //                        TurnOffDetector2?.Invoke(this, sensorCode);
        //                    }

        //                    break;
        //                }

        //            case SensorCode.S5_ON_FWD:
        //                {
        //                    nBagsPartiallyInsideTunnel += 1;
        //                    nBagsFullyInsideTunnel -= 1;
        //                    LogBagData();
        //                    break;
        //                }

        //            case SensorCode.S5_OFF_FWD:
        //                {
        //                    if (IsSourceOff)
        //                    {
        //                        _logger.LogWarning($"[{sensorCode}:{(int)sensorCode}]: Source SHOULD NOT have been already OFF at this state. This state is alarming.");
        //                        if (IsBagInTunnel)
        //                        {
        //                            _logger.LogCritical($"[{sensorCode}:{(int)sensorCode}]: Source is Off but the bag(s) is/are partially or fully inside the tunnel.");
        //                            LogBagData();
        //                            throw new Exception("Source is off but bag(s) is/are partially or fully inside the tunnel");
        //                        }

        //                        nBagsPartiallyInsideTunnel -= 1;
        //                        _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: {TotalBagsInsideTunnel} Bag(s) inside the tunnel.");
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        if (IsBagFullyInsideTunnel)
        //                        {
        //                            nBagsPartiallyInsideTunnel -= 1;
        //                            LogBagData();
        //                        }
        //                        else
        //                        {
        //                            IsSourceOn = false;
        //                            nBagsPartiallyInsideTunnel -= 1;
        //                            LogBagData();
        //                            _logger.LogInformation($"[{sensorCode}:{(int)sensorCode}]: Source is turning OFF ...");
        //                            TurnOffSource?.Invoke(this, sensorCode);
        //                        }
        //                        break;
        //                    }
        //                }

        //            default:
        //                if (_suppressInvalidValueException)
        //                {
        //                    _logger.LogInformation($"[{typeof(SensorCode)}:{typeof(XRayMachineStatusManager)}]: Invalid value: {sensorCode}");
        //                }
        //                else
        //                {
        //                    throw new ArgumentException($"[{typeof(SensorCode)}:{typeof(XRayMachineStatusManager)}]: Invalid value: {sensorCode}");
        //                }

        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error processing status: {ex.Message}");
        //    }
        //}

        private void LogBagData()
        {

            _logger.LogInformation($"Bags Fully Inside: {nBagsFullyInsideTunnel}");
            _logger.LogInformation($"Bags Partially Inside: {nBagsPartiallyInsideTunnel}");
        }


        private bool IsBagFullyInsideTunnel =>
            nBagsFullyInsideTunnel > 0;

        private bool IsBagPartiallyInsideTunnel =>
            nBagsPartiallyInsideTunnel > 0;

        private bool IsBagInTunnel =>
            (IsBagFullyInsideTunnel || IsBagPartiallyInsideTunnel);

        private bool IsSourceOff =>
            !IsSourceOn;

        private int TotalBagsInsideTunnel =>
            nBagsFullyInsideTunnel + nBagsPartiallyInsideTunnel;

        private int nBagsFullyInsideTunnel = 0;
        private int nBagsPartiallyInsideTunnel = 0;
        private bool IsSourceOn = false;
        private bool IsDetector1_On = false;
        private bool IsDetector2_On = false;
    }
}
