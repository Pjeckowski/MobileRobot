using System.IO.Ports;
using System.Text;
using System.Windows;
using MobileRobotControl.RobotPacket;
using MobileRobotControl.RobotStatusUpdate;

namespace MobileRobotControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private Connection_Window connectionWindow;
        private RS232 rs232;

        public MainWindow()
        {
            InitializeComponent();
            robotPosXLabel.Content = sizeof(float).ToString();
            byte[] b = {80, 1, 1, 1, 1};

            string a = Encoding.ASCII.GetString(b);
            IRobotPacket packet = new RobotPacket.RobotPacket(a);
            RobotStatusUpdateFactory factory  = new RobotStatusUpdateFactory();
            IRobotStatusUpdate robotStatusUpdate = factory.GetRobotStatusUpdate(packet);
            if (robotStatusUpdate.GetType() == typeof(XPosUpdate))
            {
                robotAngleLabel.Content = "Działa!! :D:D";
            }
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            connectionWindow = new Connection_Window();
            connectionWindow.Owner = this;
            connectionWindow.Port_Open += PortOpenRequest;
            connectionWindow.Show();
        }

        private void PortOpenRequest(SerialPort port)
        {
            try
            {
                rs232.close();
            }
            catch{}
            rs232 = new RS232();
            rs232.PortOpen(port);
            rs232.DataReceived += Rs232DataReceived;
            rs232.DataSent += Rs232DataSent;
        }

        private void Rs232DataSent(string data)
        {
            throw new System.NotImplementedException();
        }

        private void Rs232DataReceived(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}
