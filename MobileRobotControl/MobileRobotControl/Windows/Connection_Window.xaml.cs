using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Ports;

namespace MobileRobotControl
{
    /// <summary>
    /// Interaction logic for Connection_Window.xaml
    /// </summary>
    public partial class Connection_Window : Window
    {
        bool PortsDetected;
        SerialPort Port;
        public delegate void PS(SerialPort Port);
        public PS Port_Open;

        public Connection_Window()
        {
            
            InitializeComponent();
            Connection_Window1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            string[] portnames= SerialPort.GetPortNames();

            if (portnames.Length < 1)
            {
                MessageBox.Show("No serial ports available! Diplayed ports can be used only for connection predefine and port cannot be openned now.");
                for (int i = 0; i <= 20; i++)
                {
                    Port_CB.Items.Add("COM" + i.ToString());
                }
                PortsDetected = false;
            }
            else
            {
                foreach(string portname in portnames)
                {
                    Port_CB.Items.Add(portname);
                }
                PortsDetected = true;
            }
            if (Port_CB.Items.Contains(Properties.Settings.Default.MPortName))
            {
                Port_CB.SelectedItem = Properties.Settings.Default.MPortName;
            }
            else
            Port_CB.SelectedIndex=0;



            Speed_CB.Items.Add("9600");
            Speed_CB.Items.Add("19200");
            Speed_CB.Items.Add("38400");
            Speed_CB.Items.Add("57600");

            if (Speed_CB.Items.Contains(Properties.Settings.Default.MBaudRate))
            {
                Speed_CB.SelectedItem = Properties.Settings.Default.MBaudRate;
            }
            else
                Speed_CB.SelectedIndex = 0;

            DataBits_CB.Items.Add("7");
            DataBits_CB.Items.Add("8");

            if (DataBits_CB.Items.Contains(Properties.Settings.Default.MDataBits))
            {
                DataBits_CB.SelectedItem = Properties.Settings.Default.MDataBits;
            }
            else
                DataBits_CB.SelectedIndex=1;

            Parity_CB.Items.Add("None");
            Parity_CB.Items.Add("Even");
            Parity_CB.Items.Add("Odd");

            if (Parity_CB.Items.Contains(Properties.Settings.Default.MParity))
            {
                Parity_CB.SelectedItem = Properties.Settings.Default.MParity;
            }
            else
            Parity_CB.SelectedIndex=1;

            StopBits_CB.Items.Add("1");
            StopBits_CB.Items.Add("2");

            if (StopBits_CB.Items.Contains(Properties.Settings.Default.MStopBits))
            {
                StopBits_CB.SelectedItem = Properties.Settings.Default.MStopBits;
            }
            else
            StopBits_CB.SelectedIndex=1;

            DataBits_CB.MouseEnter += Combobox_MouseEnter;
            Parity_CB.MouseEnter += Combobox_MouseEnter;
            Port_CB.MouseEnter += Combobox_MouseEnter;
            Speed_CB.MouseEnter += Combobox_MouseEnter;
            StopBits_CB.MouseEnter += Combobox_MouseEnter;

            DataBits_CB.MouseLeave += Combobox_MouseLeave;
            Parity_CB.MouseLeave += Combobox_MouseLeave;
            Port_CB.MouseLeave += Combobox_MouseLeave;
            Speed_CB.MouseLeave += Combobox_MouseLeave;
            StopBits_CB.MouseLeave += Combobox_MouseLeave;
            

        }

        private void SetOpen_Button_Click(object sender, RoutedEventArgs e)
        {
            if (PortsDetected)
            {
                if (Parity_CB.SelectedIndex == 1 && StopBits_CB.SelectedIndex == 1 && DataBits_CB.SelectedIndex == 1 && Speed_CB.SelectedIndex == 0)
                {
                    Port = new SerialPort(Port_CB.SelectedItem.ToString(), 9600, Parity.Even, 8, StopBits.Two);
                    Settings_Save();
                    if(null!= Port_Open)
                    Port_Open(Port);
                }
                else
                {
                    MessageBox.Show("Chosen port's parameters doesn't meet robot's requirements. Even best program can't stop dumb ppl from doing dumb things!");
                    Port = new SerialPort();
                    Port.BaudRate = Convert.ToInt32(Speed_CB.SelectedItem.ToString());
                    
                    if (Parity_CB.SelectedItem.ToString() == "None")
                    {
                        Port.Parity = Parity.None;
                    }
                    else
                        if (Parity_CB.SelectedItem.ToString() == "Even")
                        {
                            Port.Parity = Parity.Even;
                        }
                        else
                            if (Parity_CB.SelectedItem.ToString() == "Odd")
                            {
                                Port.Parity = Parity.Odd;
                            }
                    if (DataBits_CB.SelectedItem.ToString() == "7")
                    {
                        Port.DataBits = 7;
                    }
                    else
                        if (DataBits_CB.SelectedItem.ToString() == "8")
                        {
                            Port.DataBits = 8;
                        }

                    if (StopBits_CB.SelectedItem.ToString() == "1")
                    {
                        Port.StopBits = StopBits.One;
                    }
                    else
                        Port.StopBits = StopBits.Two;

                    Settings_Save();
                    if(null != Port_Open)
                        Port_Open(Port);
                }
            }
            else
            {
                MessageBox.Show("Port cannot be openned because no available ports have been detected. Connect the device and open connection window again.");
            }
        }

        private void Set_Button_Click(object sender, RoutedEventArgs e)
        {
            Settings_Save();
            this.Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Combobox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender.Equals(DataBits_CB))
            {
                DataBits_CB.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (sender.Equals(Parity_CB))
            {
                Parity_CB.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (sender.Equals(Port_CB))
            {
                Port_CB.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (sender.Equals(Speed_CB))
            {
                Speed_CB.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (sender.Equals(StopBits_CB))
            {
                StopBits_CB.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void Combobox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender.Equals(DataBits_CB))
            {
                DataBits_CB.Foreground = new SolidColorBrush(Colors.White);
            }
            if (sender.Equals(Parity_CB))
            {
                Parity_CB.Foreground = new SolidColorBrush(Colors.White);
            }
            if (sender.Equals(Port_CB))
            {
                Port_CB.Foreground = new SolidColorBrush(Colors.White);
            }
            if (sender.Equals(Speed_CB))
            {
                Speed_CB.Foreground = new SolidColorBrush(Colors.White);
            }
            if (sender.Equals(StopBits_CB))
            {
                StopBits_CB.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void Settings_Save()
        {
            Properties.Settings.Default.MBaudRate = Speed_CB.SelectedItem.ToString();
            Properties.Settings.Default.MPortName = Port_CB.SelectedItem.ToString();
            Properties.Settings.Default.MDataBits = DataBits_CB.SelectedItem.ToString();
            Properties.Settings.Default.MParity = Parity_CB.SelectedItem.ToString();
            Properties.Settings.Default.MStopBits = StopBits_CB.SelectedItem.ToString();

            Properties.Settings.Default.Save();
        }

    }
}
