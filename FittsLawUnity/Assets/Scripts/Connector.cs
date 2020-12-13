using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Task;
////using System.IO.Ports;


namespace WheelchairSerialConnect
{
    public class Movement
    {
        //Ticks per circle = 1024
        //Wheel diameter = 6.00 cm (60000 micrometer)
        private const double ticks2MicroMeterFactor = 1;//60000/1024;   
        
        public int LeftTicks {
            private set;
            get;
        }

        public int RightTicks
        {
            private set;
            get;
        }

        public Movement(int lTicks, int rTicks)
        {
            LeftTicks = lTicks;
            RightTicks = rTicks;
        }

        internal void IncreaseWith(Movement m)
        {
            LeftTicks += m.LeftTicks;
            RightTicks += m.RightTicks;
        }

        internal double getLeftMicroMeter()
        {
            return LeftTicks * ticks2MicroMeterFactor;
        }

        internal double getRightMicroMeter()
        {
            return RightTicks * ticks2MicroMeterFactor;
        }
        internal void Reset()
        {
            LeftTicks = 0;
            RightTicks = 0;
        }

    }

    public class Connector
    {
        //Msg format: "wc:<left value>,<right value>;"
        private static readonly String END_CHAR = ";";
        private static readonly String COL_CHAR = ":";
        private static readonly String SEP_CHAR = ",";
        private static readonly String START_REQUEST = "s?" + END_CHAR;
        private static readonly String START_RESPONSE = "wc" + COL_CHAR;
        private static readonly String DATA_REQUEST = "d" + END_CHAR;
        private static readonly long RESPONSE_TIME_MS = 1000;



        Action<String> collectorAction = delegate (String s)
        {
            interpretIncoming(s); //TODO Implement actual method here...
        };

        ////Action<SerialPort> collector = delegate (SerialPort sp)
        ////{
        ////    while(true)//sp.IsOpen)
        ////        interpretIncoming(sp.ReadLine()); //TODO Implement actual method here...
        ////};

        //private static Thread collectorThread = new Thread() {
        //};

        private static Connector _instance;
        ////private static SerialPort _serialPort;
      //  public event EventHandler OnMove;

        //private static Movement _latestMovement;
        private static String incomingLeftover;
        

        private Connector()
        {
            //Singleton pattern
        }


        public Boolean AutoConnect()
        {
            String[] ports = new String[] { "COM5" };// SerialPort.GetPortNames();

            Console.Out.WriteLine("Autoconnecting - found the following ports:");
            StringBuilder sb = new StringBuilder();
            if (ports.Length < 1)
            {
                sb.Append("<none found>");
            }
            else
            {
                foreach (String port in ports)
                    sb.AppendLine(port);
            }
            Console.Out.Write(sb.ToString());

            //TODO tryDisconnect(); //Make sure it is disconnected and that we don't leave an open connection!
            ////foreach (String port in ports)
            ////{
            ////    if (!port.Equals("COM1"))
            ////    {
            ////        SerialPort sp = tryConnect(port, 9600, Parity.None, 8, StopBits.One);   //_serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            ////        if (sp != null)
            ////        {
            ////            _serialPort = sp;

            ////            //  _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            ////            //collector(sp)
            ////               return true;
            ////        }
            ////    }
            
            ////}


            return false;

        }

        public Movement Collect()
        {
            ////if (_serialPort != null)
            ////{
            ////    _serialPort.WriteLine(DATA_REQUEST);
            ////    String s = _serialPort.ReadLine();

            ////    Movement m = interpretIncoming(s);
            ////    return m;// _latestMovement.IncreaseWith(m);
            ////}

            Console.Out.WriteLine("Serialport is null");
            return new Movement(0, 0);
        }

      /*  public Movement GetLatestMovement()
        {
            Movement m = new Movement(_latestMovement.LeftTicks, _latestMovement.RightTicks);
            _latestMovement.Reset();

            return m;
        }*/

        public Movement GetMovement()
        {
            //StringBuilder sb = new StringBuilder();
            ////String s = _serialPort.ReadLine();

            /* while (!String.IsNullOrEmpty(s))
             {
                 sb.Append(s);
                 s = _serialPort.ReadLine();
             }*/



            ////Movement m = /*new Movement(1, 2);/*/ interpretIncoming(s);
            ////return m;
            return new Movement(1, 2);

            /*_latestMovement.IncreaseWith(m);

            //     Console.Out.WriteLine("Parsed: (" + m.Left + "," + m.Right + ")");

            EventHandler handler = OnMove;
            if (handler != null)
                handler(m, EventArgs.Empty);*/
        }

        /*private void DataReceivedHandler(Object o, SerialDataReceivedEventArgs args)
        {
            SerialPort sp = (SerialPort)o;
            //Console.Out.WriteLine("Incoming: " + sp.ReadExisting());
            
            Movement m = interpretIncoming(sp.ReadExisting());
            _latestMovement.IncreaseWith(m);

       //     Console.Out.WriteLine("Parsed: (" + m.Left + "," + m.Right + ")");

            EventHandler handler = OnMove;
            if (handler != null)
                handler(m, EventArgs.Empty);
        }*/

        private static Movement interpretIncoming(String str)
        {
            str += incomingLeftover;
            int l = 0;
            int r = 0;

            int end = str.IndexOf(END_CHAR);
            while (-1 < end)
            {
                int colIndex = str.IndexOf(COL_CHAR);
                int index = str.IndexOf(SEP_CHAR);

                try
                {
                    //TODO If this fails it will turn into an constant loop... :-/
                    int tl = Int32.Parse(str.Substring(colIndex+1, index-colIndex-1));
                    int tr = Int32.Parse(str.Substring(index+1, end-index-1));

                    //Remove parsed bit
                    str = str.Substring(end + 1);

                    //If both parsed => add to counters
                    l += tl;
                    r += tr;
                } catch (FormatException)
                {
                    //Something went wrong during the parsing... Not doing anything about that.
                    Console.Error.WriteLine("Unable to parse incoming substring: \"" + str.Substring(0, end) + "\"");
                    continue;
                }

                end = str.IndexOf(END_CHAR);
            }

            incomingLeftover = str;

            return new Movement(l, r*-1); 

        }

        
        ////private SerialPort tryConnect(String portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        ////{
        ////    SerialPort sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        ////    try
        ////    {
        ////        sp.Open();
        ////    }
        ////    catch (UnauthorizedAccessException)
        ////    {
        ////        Console.WriteLine(portName + " occupied");
        ////        return null;
        ////    }
        ////    catch (IOException ex)
        ////    {
        ////        Console.WriteLine("Not connected");
        ////        return null;
        ////    }

        ////    //sp.Write(START_REQUEST);
        ////    sp.Write(DATA_REQUEST);
        ////    DateTime end = DateTime.Now.AddMilliseconds(RESPONSE_TIME_MS);
        ////    String incoming = "";
        ////    do
        ////    {
        ////        //incoming += sp.ReadExisting();
        ////        incoming += sp.ReadLine();
        ////        if (incoming.Contains(START_RESPONSE))
        ////        {
        ////            Console.WriteLine(portName + ": Connected!");
        ////            return sp;
        ////        }
        ////    } while (DateTime.Now.CompareTo(end) < 0);

        ////    Console.WriteLine("Incoming: " + incoming);
        ////    Console.WriteLine(portName + ": No valid response");
        ////    return null;
        ////}

        public void Disconnect()
        {
            ////if (_serialPort == null)
            ////    return;

            ////_serialPort.Close();
            ////_serialPort = null;
        }





        public static Connector getInstance()
        {
            if (_instance == null)
                _instance = new Connector();
            return _instance;
        }
    }
}
