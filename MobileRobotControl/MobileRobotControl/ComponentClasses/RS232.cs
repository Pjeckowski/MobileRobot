using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text;

namespace MobileRobotControl
{
    public class RS232
    {
        string data;
        private SerialPort Port;
        public delegate void RSDataReceivedEvent(string data);
        public RSDataReceivedEvent DataReceived, DataSent;
        public bool UploadInProgress { get; private set; }

        public bool PortOpen()
        {
            String[] PortsAvailable = SerialPort.GetPortNames();
            if (PortsAvailable.Length != 0)
            {
                Port = new SerialPort()
                {
                    PortName = PortsAvailable[0],
                    BaudRate = 9600,
                    Parity = Parity.Even,
                    DataBits = 8,
                    StopBits = StopBits.Two
                };
                Port.RtsEnable = true;
                Port.Encoding = Encoding.GetEncoding("iso-8859-1");
                Port.DataReceived += new SerialDataReceivedEventHandler(ReceiverHandler);
                Port.Open();

                data = "";
                return true;
            }
            return false;
        }

        public bool PortOpen(SerialPort Port)
        {

            this.Port = new SerialPort();
            this.Port = Port;
            this.Port.RtsEnable = true;
            Port.Encoding = Encoding.GetEncoding("iso-8859-1");
            this.Port.DataReceived += new SerialDataReceivedEventHandler(ReceiverHandler);

            try
            {
                this.Port.Open();
                data = "";
                return true;
            }
            catch (Exception)
            {

                if (this.Port.IsOpen)
                {
                    return true;
                }
                return false;
            }
                
        }

        void ReceiverHandler(object sender,SerialDataReceivedEventArgs e)
        {
            data += Port.ReadExisting();
            
            if (data.Contains("\r"))
            {
                data = data.Replace(('\r').ToString(), "");
                if(DataReceived != null) DataReceived(data);
                data = "";
            }
        }

        public bool send(string data)
        {
            try
            {
                Port.Write(data + "\r");
                if (DataSent != null)
                {
                    DataSent(data);
                }
                return true;
            }
            catch
            {
                return false;
            }

            //port.Write(data+ "\r");
        }


        private async void Upload(List<String> Data)
        {
            int Count = Data.Count;
            int i=0;
            while (UploadInProgress == true && i<Count)
            {
                send(Data[i++]);
                await Task.Delay(500);
            }
        }
        public bool Send(List<String> Data)
        {
           if (UploadInProgress)
            {
                return false;
            }
           else
            {
                Upload(Data);
                return true;
            }

        }
         public void close()
        {
            Port.Dispose();
            Port.Close();
        }
        
        public bool isopen()
        {
            return Port.IsOpen;
        }

    }
}
