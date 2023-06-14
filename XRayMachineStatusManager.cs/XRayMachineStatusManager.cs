using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging.TraceSource;
//using XRayMachineStatusManager.cs;

namespace XRayMachineStatusManagement
{
    public class XRayMachineStatusManager 
    {
        private readonly ConsoleLogger _logger;
        
        private readonly bool _suppressInvalidValueException;

        public event EventHandler<int> TurnOnSource;
        public event EventHandler<int> TurnOffSource;
        public event EventHandler<int> TurnOnDetector1;
        public event EventHandler<int> TurnOffDetector1;
        public event EventHandler<int> TurnOnDetector2;
        public event EventHandler<int> TurnOffDetector2;


        public XRayMachineStatusManager()
        {
            _suppressInvalidValueException = false;
            _logger = new ConsoleLogger();
        }

        public async Task DecideStatusAsync(int sensorCode)
        {
            try
            {
                await Task.Delay(100);

                switch (sensorCode)
                {
                    case 4849: //FWD:S1_ON
                        if (IsSourceOn)
                        {
                            _logger.LogInformation($"[S1_ON:{sensorCode}]:Source Already ON.");
                            nBagsPartiallyInsideTunnel += 1;
                            LogBagData();
                            break;
                        }
                        else
                        {
                            IsSourceOn = true;
                            nBagsPartiallyInsideTunnel += 1;
                            _logger.LogInformation($"[S1_ON:{sensorCode}]:Source Turned ON.");
                            LogBagData();
                            TurnOnSource?.Invoke(this, sensorCode);
                            break;
                        }

                    case 5849: //FWD:S1_OFF
                        {
                            nBagsPartiallyInsideTunnel -= 1;
                            nBagsFullyInsideTunnel += 1;
                            _logger.LogInformation($"[S1_OFF:{sensorCode}]: {TotalBagsInsideTunnel} Bag(s) inside the tunnel.");
                            break;
                        }

                    case 4865://FWD:S5_ON
                        {
                            nBagsPartiallyInsideTunnel += 1;
                            nBagsFullyInsideTunnel -= 1;
                            LogBagData();
                            break;
                        }

                    case 5865: //FWD:S5_OFF
                        {
                            if (IsSourceOff)
                            {
                                _logger.LogWarning($"[S5_OFF:{sensorCode}]: Source SHOULD NOT have been already OFF at this state. This state is alarming.");
                                if (IsBagInTunnel)
                                {
                                    _logger.LogCritical($"[S5_OFF:{sensorCode}]: Source is Off but the bag(s) is/are partially or fully inside the tunnel.");
                                    LogBagData();
                                    throw new Exception("Source is off but bag(s) is/are partially or fully inside the tunnel");
                                }

                                nBagsPartiallyInsideTunnel -= 1;
                                _logger.LogInformation($"[S5_OFF:{sensorCode}]: {TotalBagsInsideTunnel} Bag(s) inside the tunnel.");
                                break;
                            }
                            else
                            {
                                if (IsBagFullyInsideTunnel)
                                {
                                    nBagsPartiallyInsideTunnel -= 1;
                                    LogBagData();
                                }
                                else
                                {
                                    IsSourceOn = false;
                                    nBagsPartiallyInsideTunnel -= 1;
                                    LogBagData();
                                    TurnOffSource?.Invoke(this, sensorCode);
                                }
                                break;
                            }
                        }

                    case 4841://FWD:S2_ON
                        {
                            if (IsDetector1_On)
                            {
                                _logger.LogInformation($"[S2_ON:{sensorCode}]: Detector 1 is already ON.");
                            }
                            else
                            {
                                IsDetector1_On = true;
                                _logger.LogInformation($"[S2_ON:{sensorCode}]: Detector 1 is turned ON.");
                                TurnOnDetector1?.Invoke(this, sensorCode);
                            }
                            break;
                        }

                    case 5841://FWD:S2_OFF
                        //Todo: Add fully/partially inside for detection.
                        _logger.LogInformation($"[S2_OFF:{sensorCode}]: Noted Un-used Sensor Signal.");
                        break;

                    case 4844://FWD:S3_ON
                        if (IsDetector2_On)
                        {
                            _logger.LogInformation($"[S3_ON:{sensorCode}]: Detector 2 is already ON.");
                        }
                        else
                        {
                            IsDetector1_On = true;
                            _logger.LogInformation($"[S3_ON:{sensorCode}]: Detector 2 is turned ON.");
                            TurnOnDetector1?.Invoke(this, sensorCode);
                        }
                        break;

                    case 4843://FWD:S3_OFF
                        //Todo: Add fully/partially inside for detection.
                        _logger.LogInformation($"[S3_OFF:{sensorCode}]: Un-used Sensor Signal.");
                        break;

                    case 5845://FWD:S4_ON
                        //Todo: Add fully/partially inside for detection.
                        _logger.LogInformation($"[S4_ON:{sensorCode}]: Un-used Sensor Signal.");
                        break;

                    case 4845://FWD:S4_OFF
                        {
                            if (IsDetector1_On)
                            {
                                IsDetector1_On = false;
                                _logger.LogInformation($"[S4_OFF:{sensorCode}]: Detector 1 is turned OFF.");
                                TurnOffDetector1?.Invoke(this, sensorCode);
                            }
                            if (IsDetector2_On)
                            {
                                IsDetector2_On = false;
                                _logger.LogInformation($"[S4_OFF:{sensorCode}]: Detector 2 is turned OFF.");
                                TurnOffDetector2?.Invoke(this, sensorCode);
                            }

                            break;
                        }

                    default:
                        if (_suppressInvalidValueException)
                        {
                            _logger.LogInformation($"Invalid value: {sensorCode}");
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid value: {sensorCode}", nameof(sensorCode));
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
