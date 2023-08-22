using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using XRayMachineStatusManagement;

namespace SensorData
{
    internal class SensorDataSequence
    {
        System.IO.Ports.SerialPort KeyBoardSerialPort;
        System.IO.Ports.SerialPort USBPort;
        XRayMachineStatusManagement.XRayMachineStatusManager manager;

        System.Diagnostics.Stopwatch beltStopWatch = new System.Diagnostics.Stopwatch();
        public System.Diagnostics.Stopwatch IsConveyorBeltOnOROff = new System.Diagnostics.Stopwatch();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        int count = 0;
        public SensorDataSequence()
        {
            manager = new XRayMachineStatusManagement.XRayMachineStatusManager();
            
            manager.TurnOnSource += Manager_TurnOnSource;
            manager.TurnOffSource += Manager_TurnOffSource;
            manager.TurnOnDetector1 += Manager_TurnOnDetector1;
            manager.TurnOffDetector1 += Manager_TurnOffDetector1;
            manager.TurnOnDetector2 += Manager_TurnOnDetector2;
            manager.TurnOffDetector2 += Manager_TurnOffDetector2;
            manager.SourceOffExceptionalEvent += Manager_SourceOffExceptionalEvent;

            this.KeyBoardSerialPort = new System.IO.Ports.SerialPort(new System.ComponentModel.Container());
            this.USBPort = new System.IO.Ports.SerialPort(new System.ComponentModel.Container());
            InitializeKeyboardSerialPort();
            InitializeKeyboardUSBPort();

        }

        private void Manager_SourceOffExceptionalEvent(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffSource)}] EventHandled -->> Source is Off");
        }

        private void Manager_TurnOnDetector2(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnDetector2)}] EventHandled -->> Detector 2 is On");
        }

        private void Manager_TurnOffDetector2(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffDetector2)}] EventHandled -->> Detector 2 is Off");
        }

        private void Manager_TurnOffDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffDetector1)}] EventHandled -->> Detector 1 is Off");
        }

        private void Manager_TurnOnDetector1(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnDetector1)}] EventHandled -->> Detector 1 is On");
        }

        private void Manager_TurnOffSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOffSource)}] EventHandled -->> Source is Off");
        }

        private void Manager_TurnOnSource(object sender, SensorCode e)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] [{nameof(Manager_TurnOnSource)}] EventHandled -->> Source is On");
        }

        public void InitializeKeyboardSerialPort()
        {
            try
            {
                if (KeyBoardSerialPort.IsOpen)
                {
                    KeyBoardSerialPort.Close();
                }
                KeyBoardSerialPort.DtrEnable = true;
                KeyBoardSerialPort.BaudRate = 115200;
                KeyBoardSerialPort.DataBits = 8;
                KeyBoardSerialPort.Parity = Parity.None;
                KeyBoardSerialPort.StopBits = StopBits.One;
                KeyBoardSerialPort.Handshake = Handshake.None;
                KeyBoardSerialPort.PortName = "COM12";
                KeyBoardSerialPort.Open();

                KeyBoardSerialPort.DataReceived += new SerialDataReceivedEventHandler(keyboard_data_receivedSingleView);

            }
            catch (Exception ex)
            {
                log.Error("InitializeKeyboardSerialPort:" + ex.Message);
            }

        }

        public void InitializeKeyboardUSBPort()
        {
            try
            {
                if (USBPort.IsOpen)  // If serial port is not open close it it
                {
                    USBPort.Close();
                }
                USBPort.DtrEnable = true;
                USBPort.BaudRate = 115200;
                USBPort.DataBits = 8;
                USBPort.Parity = Parity.None;
                USBPort.StopBits = StopBits.None;
                USBPort.Handshake = Handshake.None;
                USBPort.PortName = "COM13";
                USBPort.Open();

                USBPort.DataReceived += new SerialDataReceivedEventHandler(USBPort_DataReceived);

            }
            catch (Exception ex)
            { }

        }

        DateTime prevSensorTimeStamp = DateTime.Now;
        DateTime newsensorTimeStamp = DateTime.Now;

        private void keyboard_data_receivedSingleView(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                newsensorTimeStamp = DateTime.Now;
                TimeSpan delayDuration = newsensorTimeStamp - prevSensorTimeStamp;
                prevSensorTimeStamp = newsensorTimeStamp;
                

                int newNumber = 0;
                string readSerial = KeyBoardSerialPort.ReadExisting();
                if (readSerial.Length % 10 == 0 && readSerial != "" || readSerial.Length == 10)
                {
                    int div = readSerial.Length / 10;
                    char[] KeyboardString1 = readSerial.ToCharArray();
                    for (int j = 0; j < div; j++)
                    {
                        char[] KeyboardString = new Char[10];
                        for (int k = 0; k < 10; k++)
                        {
                            KeyboardString[k] = KeyboardString1[j * 10 + k];
                        }
                        int initial = 0;
                        for (int i = 0; i < (KeyboardString.Length - 3); i++)
                        {
                            initial ^= KeyboardString[i];
                        }
                        if (initial == KeyboardString[7])
                        {
                            int value = KeyboardString[5];//Get Value of 5th from ARRAY.
                            int value2 = KeyboardString[6];//Get value of 6th from ARRAY.
                            newNumber = int.Parse(value.ToString("00") + value2.ToString("00"));
                            //Console.WriteLine("\n Serial Port command :  " + newNumber); 
                        }
                        else
                        {
                            log.Info("Checksum not matched invalid command recieved: ");
                        }

                        switch (newNumber)
                        {
                            case 4849:
                                SendAck_KB(48, 49);
                                break;

                            case 5849:
                                SendAck_KB(58, 49);
                                break;

                            case 4841:
                                SendAck_KB(48, 41);
                                break;

                            case 5841:
                                SendAck_KB(58, 41);
                                break;

                            case 4844:
                                SendAck_KB(48, 44);
                                break;

                            case 4843:
                                SendAck_KB(48, 43);
                                break;

                            case 4845:
                                SendAck_KB(48, 45);
                                break;

                            case 5845:
                                SendAck_KB(58, 45);
                                break;

                            case 4865:
                                SendAck_KB(48, 65);
                                break;

                            case 5865:
                                SendAck_KB(58, 65);
                                break;

                            case 4168:
                                SendAck_KB(41, 68);
                                SendCommand_KB(41, 68);
                                //IsConveyorBeltOnOROff.Start();
                                beltStopWatch.Start();
                                break;

                            case 4169:
                                SendAck_KB(41, 69);
                                SendCommand_KB(41, 69);
                                //IsConveyorBeltOnOROff.Reset();
                                beltStopWatch.Stop();
                                break;

                            case 4170:
                                SendAck_KB(41, 70);
                                SendCommand_KB(41, 70);
                                //IsConveyorBeltOnOROff.Start();
                                beltStopWatch.Start();
                                break;
                        }

                        if (Enum.TryParse(newNumber.ToString(), out XRayMachineStatusManagement.SensorCode sensorCodeFromManager))
                        {
                            //Console.WriteLine($"Task.Delay({(int)delayDuration.TotalMilliseconds}).Wait(); manager.DecideStatus(SensorCode.{sensorCodeFromManager});");
                            manager.DecideStatus(sensorCodeFromManager);
                        }

                        //if (newNumber == 4849 || newNumber == 5849 || newNumber == 4865 || newNumber == 5865)
                        //{
                        //    return;
                        //}

                        //if (Enum.TryParse(newNumber.ToString(), out SensorCode sensorCode))
                        //    Console.WriteLine($"[{++count}] - [{sensorCode}] - [{DateTime.Now}] - [{DateTime.Now.TimeOfDay.TotalMilliseconds}] ");
                        //else
                        //    Console.WriteLine($"[{newNumber}] - [{DateTime.Now}] - [{DateTime.Now.TimeOfDay.TotalMilliseconds}] - XXX Unknown Sensor Code. XXX");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Error after keyboard_data_received: " + ex.Message);
            }
        }

        private void USBPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("USB Data Received");
            try
            {
                int newNumber = 0;
                string readSerial = USBPort.ReadExisting();
                if (readSerial.Length % 10 == 0 && readSerial != "" || readSerial.Length == 10)
                {
                    int div = readSerial.Length / 10;
                    char[] KeyboardString1 = readSerial.ToCharArray();
                    for (int j = 0; j < div; j++)
                    {
                        char[] KeyboardString = new Char[10];
                        for (int k = 0; k < 10; k++)
                        {
                            KeyboardString[k] = KeyboardString1[j * 10 + k];
                        }
                        int initial = 0;
                        for (int i = 0; i < (KeyboardString.Length - 3); i++)
                        {
                            initial ^= KeyboardString[i];
                        }
                        if (initial == KeyboardString[7])
                        {
                            int value = KeyboardString[5];//Get Value of 5th from ARRAY.
                            int value2 = KeyboardString[6];//Get value of 6th from ARRAY.
                            newNumber = int.Parse(value.ToString("00") + value2.ToString("00"));

                            Console.WriteLine("\n USB Port command :  " + newNumber);
                        }
                        else
                        {
                            log.Info("Checksum not matched invalid command received: ");
                        }

                        switch (newNumber)
                        {
                            case 4168:
                                SendAck_KB(41, 68);
                                SendCommand_KB(41, 68);
                                //IsConveyorBeltOnOROff.Start();
                                beltStopWatch.Start();
                                break;

                            case 4169:
                                SendAck_KB(41, 69);
                                SendCommand_KB(41, 69);
                                //IsConveyorBeltOnOROff.Reset();
                                beltStopWatch.Stop();
                                break;

                            case 4170:
                                SendAck_KB(41, 70);
                                SendCommand_KB(41, 70);
                                //IsConveyorBeltOnOROff.Start();
                                beltStopWatch.Start();
                                break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Error after keyboard_data_received: " + ex.Message);
            }
        }


        private void SendAck_KB(int x, int y)               // Sending Acknowledgement to Keyboard     
        {
            try
            {
                byte[] array = kBWriteACKByte(x, y);
                KeyBoardSerialPort.Write(array, 0, array.Length);
            }
            catch (Exception ex)
            {
                log.Error("Error @ SendAck_KB: " + ex.ToString());
            }
        }

        public void SendCommand_KB(int x, int y)               // Sending Command to Keyboard
        {
            try
            {
                if (KeyBoardSerialPort.IsOpen)
                {
                    byte[] array = kBWriteByte(x, y);
                    KeyBoardSerialPort.Write(array, 0, array.Length);
                    log.Warn("SendCommand_KB: " + x + y);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error @ SendCommand_KB: " + ex.Message);
            }
        }

        static int[] startIndex_ack = { 64, 36, 10, 65, 80 };
        static int[] ResultIndex_ack = new int[12];
        static int[] Checksum = new int[1];
        static int[] Checksum_ack = new int[1];
        static int[] EndIndex = { 35, 37 };
        static int[] ResultIndex = new int[12];
        static int[] arr = new int[2];
        static int[] startIndex = { 64, 36, 10, 75, 66 };

        public static byte[] kBWriteACKByte(int CadeA, int CodeB)
        {
            int[] commands = new int[] { CadeA, CodeB };
            var result1 = startIndex_ack.Concat(commands);
            ResultIndex_ack = result1.Cast<int>().ToArray();      //Integer Array Which is sent to Serial Port
            int initial = 0;
            for (int i = 0; i < ResultIndex_ack.Length; ++i)
            {
                initial ^= ResultIndex_ack[i];
            }
            Checksum_ack[0] = initial;
            var finalresult = ResultIndex_ack.Concat(Checksum_ack);
            ResultIndex_ack = finalresult.Cast<int>().ToArray();
            var ResultIndex1 = ResultIndex_ack.Concat(EndIndex);
            ResultIndex_ack = ResultIndex1.Cast<int>().ToArray();
            byte[] array = ResultIndex_ack.Select(i => (byte)i).ToArray();     //Convert Int to Byte Array as Byte array we have to send to Serial Port
            return array;
        }

        public static byte[] kBWriteByte(int CadeA, int CodeB)  //Keyboard write function converts two integer to byte array
        {
            int[] commands = new int[] { CadeA, CodeB };
            var result1 = startIndex.Concat(commands);
            ResultIndex = result1.Cast<int>().ToArray();          //Integer Array Which is sent to Serial Port
            int initial = 0;
            for (int i = 0; i < ResultIndex.Length; ++i)
            {
                initial ^= ResultIndex[i];
            }
            Checksum[0] = initial;
            var finalresult = ResultIndex.Concat(Checksum);
            ResultIndex = finalresult.Cast<int>().ToArray();
            var ResultIndex1 = ResultIndex.Concat(EndIndex);
            ResultIndex = ResultIndex1.Cast<int>().ToArray();
            byte[] array = ResultIndex.Select(i => (byte)i).ToArray();    //Convert Int to Byte Array as Byte array we have to send to Serial Port
            return array;
        }

    }
}
