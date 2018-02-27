using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;
using MobileRobotControl.Components.RobotCommunication.RobotCommands;
using MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets;
using MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.RecPacketSplitters;
using MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest;
using MobileRobotControl.Components.RobotControl;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : IRobotDataPresenter
    {
        private Connection_Window connectionWindow;
        private RS232 rs232;
        private RobotStatusUpdateFactory robotStatusUpdateFactory;
        private IRecPacketSplitter packetSplitter;
        private IPacketDescription packetDescription;
        
        delegate void UpdatePosDelegate(float value);
        private UpdatePosDelegate posUpdateDelegate;
        private object delObject;

        delegate void UpdateEngDelegate(int lEngine, int rEngine);
        private UpdateEngDelegate engUpdateDelegate;
        private object[] delObjects = new object[2];

        private IRobotCommand RobotCommand;
        private SimpleRobotControl _robotControl;

        private bool isManualControl;
        private Grid _currentlyDisplayedGrid;
        private Grid _removedGrid;

        BitmapImage _aup_True, _aup_False, _adown_True, _adown_False, _aleft_True, _aleft_False, _aright_True, _aright_False;

        public MainWindow()
        {

            InitializeComponent();
            InitImages();
            robotStatusUpdateFactory = new RobotStatusUpdateFactory();
            packetDescription = new PacketDescription("P","\r");

            RobotPosXLabel.Content = sizeof(float).ToString();

            RobotControlDockPanel.Children.Remove(ManualControlGrid);
            _currentlyDisplayedGrid = PointFollowGrid;
            _robotControl = new SimpleRobotControl();
            RobotSpeedSlider.Focusable = false;
            SetSpeedButton.Focusable = false;
            RobotStopButton.Focusable = false;
            SendControl();
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            connectionWindow = new Connection_Window {Owner = this};
            connectionWindow.Port_Open += PortOpenRequest;
            connectionWindow.Show();
        }

        private void PortOpenRequest(SerialPort port)
        {
            try
            {
                rs232.Close();
            }
            catch{}

            rs232 = new RS232();
            rs232.PortOpen(port);
            packetSplitter = new RecPacketSplitter(packetDescription, rs232);
            packetSplitter.PacketReceivedEvent += PacketReceived;
        }

        private void PacketReceived(object sender, string data)
        {
            IRecRobotPacket packet = new RecRobotPacket(data);
            IRobotStatusUpdate robotStatusUpdate = robotStatusUpdateFactory.GetRobotStatusUpdate(packet);
            robotStatusUpdate.Execute(this);
        }

#region PositionUpdates
        public void UpdateX(float value)
        {
            if (RobotPosXLabel.Dispatcher.CheckAccess())
            {
                RobotPosXLabel.Content = value.ToString();
            }
            else
            {
                delObject = value;
                posUpdateDelegate = UpdateX;
                Dispatcher.BeginInvoke(posUpdateDelegate, delObject);
            }
        }

        public void UpdateY(float value)
        {
            if (RobotPosYLabel.Dispatcher.CheckAccess())
            {
                RobotPosYLabel.Content = value.ToString();
            }
            else
            {
                delObject = value;
                posUpdateDelegate = UpdateY;
                Dispatcher.BeginInvoke(posUpdateDelegate, delObject);
            }
        }

        public void UpdateAngle(float value)
        {
            if (RobotAngleLabel.Dispatcher.CheckAccess())
            {
                RobotAngleLabel.Content = value.ToString();
            }
            else
            {
                delObject = value;
                posUpdateDelegate = UpdateAngle;
                Dispatcher.BeginInvoke(posUpdateDelegate, delObject);
            }
        }

        public void UpdateEngines(int lEngine, int rEngine)
        {
            if (leftEngineProgressBar.Dispatcher.CheckAccess())
            {
                leftEngineProgressBar.Value = lEngine;
                rightEngineProgressBar.Value = rEngine;
            }
            else
            {
                delObjects[0] = lEngine;
                delObjects[1] = rEngine;
                engUpdateDelegate = UpdateEngines;
                Dispatcher.BeginInvoke(engUpdateDelegate, delObjects);
            }
        }
#endregion

        private void SetXButton_Click(object sender, RoutedEventArgs e)
        {
            RobotCommand = new SetGoalXCommand((float) 40.0, packetDescription);
            PosXLabel.Content = RobotCommand.Content;
            RobotCommand.Execute(rs232);
        }

        private async void ManualControlButton_Click(object sender, RoutedEventArgs e)
        {
            RobotControlDockPanel.Children.Add(ManualControlGrid);
 
            int ssize = (int)_currentlyDisplayedGrid.ActualHeight / 20;
            while (_currentlyDisplayedGrid.ActualHeight > 0)
            {
                if (_currentlyDisplayedGrid.ActualHeight < 2 * ssize)
                {
                    _currentlyDisplayedGrid.Height = 0;
                    ManualControlGrid.Height = Double.NaN;
                }
                else
                {
                    _currentlyDisplayedGrid.Height = (int)_currentlyDisplayedGrid.ActualHeight - ssize;
                    ManualControlGrid.Height = ManualControlGrid.ActualHeight + ssize;
                }
                    

                await Task.Delay(1);
            }
            RobotControlDockPanel.Children.Remove(_currentlyDisplayedGrid);
            _currentlyDisplayedGrid = ManualControlGrid;
            isManualControl = true;
        }

        private async void AutoControlButton_Click(object sender, RoutedEventArgs e)
        {
            SwapGridFromTop(PointFollowGrid);
            isManualControl = false;
        }

        private async Task SwapGridFromTop(Grid gridToDisplay)
        {
            RobotControlDockPanel.Children.Remove(_currentlyDisplayedGrid);
            RobotControlDockPanel.Children.Add(gridToDisplay);
            RobotControlDockPanel.Children.Add(_currentlyDisplayedGrid);
            int ssize = (int)_currentlyDisplayedGrid.ActualHeight / 20;

            while (_currentlyDisplayedGrid.ActualHeight > 1)
            {
                if (_currentlyDisplayedGrid.ActualHeight < 2 * ssize)
                {

                    _currentlyDisplayedGrid.Height = 1;
                }
                else
                {
                    RobotAngleLabel.Content = gridToDisplay.Height = (int)gridToDisplay.ActualHeight + ssize;
                    _currentlyDisplayedGrid.Height = _currentlyDisplayedGrid.ActualHeight - ssize;
                }

                await Task.Delay(1);
            }

            RobotControlDockPanel.Children.Remove(_currentlyDisplayedGrid);
            gridToDisplay.Height = Double.NaN;
            _currentlyDisplayedGrid = gridToDisplay;
        }

        private async void SendControl()
        {
            while (true)
            {
                if (rs232 != null && rs232.IsOpen && isManualControl)
                {
                    var enginesFill = _robotControl.EnginesFill;
                    RobotCommand = new SetEnginesCommand(enginesFill.LeftEngineFill, enginesFill.RightEngineFill, packetDescription);
                    RobotCommand.Execute(rs232);
                }

                await Task.Delay(200);
            }
        }


        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _robotControl.aUp = true;
                    AUpImage.Source = _aup_True;
                    break;
                case Key.Down:
                    _robotControl.aDown = true;
                    ADownImage.Source = _adown_True;
                    break;
                case Key.Left:
                    _robotControl.aLeft = true;
                    ALeftImage.Source = _aleft_True;
                    break;
                case Key.Right:
                    _robotControl.aRight = true;
                    ARightImage.Source = _aright_True;
                    break;
            }
        }

        private void Window_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _robotControl.aUp = false;
                    AUpImage.Source = _aup_False;
                    break;
                case Key.Down:
                    _robotControl.aDown = false;
                    ADownImage.Source = _adown_False;
                    break;
                case Key.Left:
                    _robotControl.aLeft = false;
                    ALeftImage.Source = _aleft_False;
                    break;
                case Key.Right:
                    _robotControl.aRight = false;
                    ARightImage.Source = _aright_False;
                    break;
            }
        }

        private void InitImages()
        {
            _aup_True = new BitmapImage();
            _aup_True.BeginInit();
            _aup_True.UriSource = new Uri("graphic/aup_active.jpg", UriKind.Relative);
            _aup_True.EndInit();
            _aup_False = new BitmapImage();
            _aup_False.BeginInit();
            _aup_False.UriSource = new Uri("graphic/aup.jpg", UriKind.Relative);
            _aup_False.EndInit();
            _adown_True = new BitmapImage();
            _adown_True.BeginInit();
            _adown_True.UriSource = new Uri("graphic/adown_active.jpg", UriKind.Relative);
            _adown_True.EndInit();
            _adown_False = new BitmapImage();
            _adown_False.BeginInit();
            _adown_False.UriSource = new Uri("graphic/adown.jpg", UriKind.Relative);
            _adown_False.EndInit();
            _aleft_True = new BitmapImage();
            _aleft_True.BeginInit();
            _aleft_True.UriSource = new Uri("graphic/al_active.jpg", UriKind.Relative);
            _aleft_True.EndInit();
            _aleft_False = new BitmapImage();
            _aleft_False.BeginInit();
            _aleft_False.UriSource = new Uri("graphic/al.jpg", UriKind.Relative);
            _aleft_False.EndInit();
            _aright_True = new BitmapImage();
            _aright_True.BeginInit();
            _aright_True.UriSource = new Uri("graphic/ar_active.jpg", UriKind.Relative);
            _aright_True.EndInit();
            _aright_False = new BitmapImage();
            _aright_False.BeginInit();
            _aright_False.UriSource = new Uri("graphic/ar.jpg", UriKind.Relative);
            _aright_False.EndInit();

            AUpImage.Source = _aup_False;
            ALeftImage.Source = _aleft_False;
            ADownImage.Source = _adown_False;
            ARightImage.Source = _aright_False;
        }
    }
}
