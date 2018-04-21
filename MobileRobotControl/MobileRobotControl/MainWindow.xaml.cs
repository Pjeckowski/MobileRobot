using System;
using System.Collections.Generic;
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
using MobileRobotControl.Windows;

namespace MobileRobotControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : IRobotDataPresenter
    {
        private Connection_Window _connectionWindow;
        private RS232 _rs232;
        private RobotStatusUpdateFactory _robotStatusUpdateFactory;
        private IRecPacketSplitter _packetSplitter;
        private IPacketDescription _packetDescription;
        
        delegate void UpdatePosDelegate(double value);
        private UpdatePosDelegate _posUpdateDelegate;

        delegate void UpdateWithIntDelegate(int value);
        private UpdateWithIntDelegate _intUpdateDelegate;
        private object _delObject;

        delegate void UpdateEngDelegate(int lEngine, int rEngine);
        private UpdateEngDelegate _engUpdateDelegate;
        private object[] _delObjects = new object[2];

        delegate void UpdateWithVoidDelegate();
        private UpdateWithVoidDelegate _updateVoidDelegate;

        private IRobotCommand _robotCommand;
        private SimpleRobotControl _robotControl;

        private bool _isManualControl;
        private Grid _currentlyDisplayedGrid;

        private DateTime previousTime;
        private int times = 0;
        private int timeinms;

        BitmapImage _aup_True, _aup_False, _adown_True, _adown_False, _aleft_True, _aleft_False, _aright_True, _aright_False;

        public MainWindow()
        {

            InitializeComponent();
            InitImages();
            _robotStatusUpdateFactory = new RobotStatusUpdateFactory();
            _packetDescription = new PacketDescription("P","\r");

            MainGrid.Height = Double.NaN;
            RobotControlDockPanel.Children.Remove(ManualControlGrid);
            RobotControlDockPanel.Children.Remove(PointFollowGrid);
            RobotControlDockPanel.Children.Remove(RobotSettingsGrid);
            RobotControlDockPanel.Children.Remove(LineFollowGrid);
            _currentlyDisplayedGrid = MainGrid;
            _robotControl = new SimpleRobotControl();
            RobotSpeedSlider.Focusable = false;
            SetSpeedButton.Focusable = false;
            RobotStopButton.Focusable = false;
            StartControlLoop();
            previousTime = DateTime.Now;
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _connectionWindow = new Connection_Window { Owner = this };
            _connectionWindow.Port_Open += PortOpenRequest;
            _connectionWindow.Show();
        }

        private void PortOpenRequest(SerialPort port)
        {
            try
            {
                _rs232.Close();
            }
            catch{}

            _rs232 = new RS232();
            _rs232.PortOpen(port);
            _packetSplitter = new RecPacketSplitter(_packetDescription, _rs232);
            _packetSplitter.PacketReceivedEvent += OnPacketReceivedEvent;
        }

        private void OnPacketReceivedEvent(object sender, string data)
        {
            IRecRobotPacket packet = new RecRobotPacket(data);
            IRobotStatusUpdate robotStatusUpdate = _robotStatusUpdateFactory.GetRobotStatusUpdate(packet);
            robotStatusUpdate.Execute(this);
            if (!(robotStatusUpdate is XPosUpdate)) return;
            var actualTime = DateTime.Now;
            var diff = actualTime - previousTime;
            previousTime = actualTime;
            timeinms += (int) diff.TotalMilliseconds;
            times++;
            if (times != 10) return;
            times = 0;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LatencyLabel.Content = timeinms/10 + "ms";
                timeinms = 0;
            }));
        }

#region PositionUpdates
        public void UpdateX(double value)
        {
            if (RobotPosXLabel.Dispatcher.CheckAccess())
            {
                RobotPosXLabel.Content = value.ToString("0.00");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateX;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateY(double value)
        {
            if (RobotPosYLabel.Dispatcher.CheckAccess())
            {
                RobotPosYLabel.Content = value.ToString("0.00");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateY;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateAngle(double value)
        {
            if (RobotAngleLabel.Dispatcher.CheckAccess())
            {
                RobotAngleLabel.Content = value.ToString("0.00");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateAngle;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateWheelSize(double value)
        {
            if (WheelSizeTextBox.Dispatcher.CheckAccess())
            {
                WheelSizeTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateWheelSize;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateWheelSpacing(double value)
        {
            if (WheelSpacingTextBox.Dispatcher.CheckAccess())
            {
                WheelSpacingTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateWheelSpacing;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateNearestWeight(double value)
        {
            if (NearestSensorWeightTextBox.Dispatcher.CheckAccess())
            {
                NearestSensorWeightTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateNearestWeight;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateMiddleWeight(double value)
        {
            if (MiddleSensorWeightTextBox.Dispatcher.CheckAccess())
            {
                MiddleSensorWeightTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateMiddleWeight;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateFahrtestWeight(double value)
        {
            if (FarthestSensorWeightTextBox.Dispatcher.CheckAccess())
            {
                FarthestSensorWeightTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateFahrtestWeight;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateFollowerKp(double value)
        {
            if (KPTextBox.Dispatcher.CheckAccess())
            {
                KPTextBox.Text = value.ToString("0.000");
            }
            else
            {
                _delObject = value;
                _posUpdateDelegate = UpdateFollowerKp;
                Dispatcher.BeginInvoke(_posUpdateDelegate, _delObject);
            }
        }

        public void UpdateFollowerTp(int value)
        {
            if (TPTextBox.Dispatcher.CheckAccess())
            {
                TPTextBox.Text = value.ToString();
            }
            else
            {
                _delObject = value;
                _intUpdateDelegate = UpdateFollowerTp;
                Dispatcher.BeginInvoke(_intUpdateDelegate, _delObject);
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
                _delObjects[0] = lEngine;
                _delObjects[1] = rEngine;
                _engUpdateDelegate = UpdateEngines;
                Dispatcher.BeginInvoke(_engUpdateDelegate, _delObjects);
            }
        }

        public void SetFollowingLineOK()
        {
            if (StartLinneFolowingButton.Dispatcher.CheckAccess())
            {
                StartLinneFolowingButton.Content = "Stop";
            }
            else
            {
                _updateVoidDelegate = SetFollowingLineOK;
                Dispatcher.BeginInvoke(_updateVoidDelegate);
            }
        }

        public void SetFollowingPointOK()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SetXButton_Click(object sender, RoutedEventArgs e)
        {
            _robotCommand = new SetGoalXCommand( 40.0, _packetDescription);
            PosXLabel.Content = _robotCommand.Content;
            _robotCommand.Execute(_rs232);
        }

#region MainGrid

        private async void ManualControlButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromTop(ManualControlGrid);
            _isManualControl = true;
        }

        private async void RobotSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromBot(RobotSettingsGrid);

            PositionXTextBox.Text = "0";
            PositionYTextBox.Text = "0";
            AngleTextBox.Text = "90";

            _robotCommand = new GetBaseParametersCommand(_packetDescription);
            await SendRobotCommand(_robotCommand);
        }

        private async void PointFollowButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromBot(PointFollowGrid);
            _isManualControl = false;
        }

        private async void LineFollowButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromTop(LineFollowGrid);

            List<IRobotCommand> robotCommands = new List<IRobotCommand>
            {
                new GetSensorWeightsCommand(_packetDescription),
                new GetLfParametersCommand(_packetDescription)
            };

            await SendRobotCommand(robotCommands);
        }

#endregion

        private async Task SendRobotCommand(List<IRobotCommand> commandsList)
        {
            try
            {
                foreach (IRobotCommand command in commandsList)
                {
                    await Task.Delay(200);
                    command.Execute(_rs232);
                }
            }
            catch (Exception e)
            {
                WarningWindow ww = new WarningWindow("Not connected!") {Owner = this};
                ww.ShowDialog();
            }
        }

        private async Task SendRobotCommand(IRobotCommand command)
        {
            await Task.Delay(200);
            try
            {
                    command.Execute(_rs232);
            }
            catch (Exception e)
            {
                WarningWindow ww = new WarningWindow("Not connected!") { Owner = this };
                ww.ShowDialog();
            }
        }

#region GridClosersRegion

        private async void BackToMainFromTopButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromTop(MainGrid);
            _isManualControl = false;
        }

        private async void BackToMainFromBotButton_Click(object sender, RoutedEventArgs e)
        {
            await SwapGridFromBot(MainGrid);
            _isManualControl = false;
        }

#endregion


        private async Task SwapGridFromBot(Grid gridToDisplay)
        {
            RobotControlDockPanel.Children.Add(gridToDisplay);

            int ssize = (int)_currentlyDisplayedGrid.ActualHeight / 20;
            while (_currentlyDisplayedGrid.ActualHeight > 0)
            {
                if (_currentlyDisplayedGrid.ActualHeight < 2 * ssize)
                {
                    _currentlyDisplayedGrid.Height = 0;
                    gridToDisplay.Height = Double.NaN;
                }
                else
                {
                    _currentlyDisplayedGrid.Height = (int)_currentlyDisplayedGrid.ActualHeight - ssize;
                    gridToDisplay.Height = gridToDisplay.ActualHeight + ssize;
                }


                await Task.Delay(1);
            }
            RobotControlDockPanel.Children.Remove(_currentlyDisplayedGrid);
            _currentlyDisplayedGrid = gridToDisplay;
        }

        private async void SetRobotSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            double x, y, angle, wheelSize, wheelSpacing;
            try
            {
                x = Convert.ToDouble(PositionXTextBox.Text);
                y = Convert.ToDouble(PositionYTextBox.Text);
                angle = Convert.ToDouble(AngleTextBox.Text);
                wheelSize = Convert.ToDouble(WheelSizeTextBox.Text);
                wheelSpacing = Convert.ToDouble(WheelSpacingTextBox.Text);
            }
            catch (Exception)
            {
                WarningWindow ww = new WarningWindow("Wrong parameters!") {Owner = this};
                ww.ShowDialog();
                return;
            }

            var robotCommands = new List<IRobotCommand>
            {
                new SetRobotPositionCommand(x, y, angle, _packetDescription),
                new SetBaseParametersCommand(wheelSize, wheelSpacing, _packetDescription),
                new GetBaseParametersCommand(_packetDescription)
            };
            await SendRobotCommand(robotCommands);

        }
        private async void StartLinneFolowingButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string) StartLinneFolowingButton.Content == "Start")
            {
                double near, mid, fahrt, kp;
                int tp;
                try
                {
                    near = Convert.ToDouble(NearestSensorWeightTextBox.Text);
                    mid = Convert.ToDouble(MiddleSensorWeightTextBox.Text);
                    fahrt = Convert.ToDouble(FarthestSensorWeightTextBox.Text);
                    tp = Convert.ToInt32(TPTextBox.Text);
                    kp = Convert.ToDouble(KPTextBox.Text);
                }
                catch (Exception)
                {
                    WarningWindow ww = new WarningWindow("Wrong parameters!"){Owner = this};
                    ww.ShowDialog();
                    return;
                }

                List<IRobotCommand> commands = new List<IRobotCommand>
                {
                    new SetLineFollowerParametersCommand(kp, tp, _packetDescription),
                    new SetReflectiveSensorWeights(near, mid, fahrt, _packetDescription),
                    new GetLfParametersCommand(_packetDescription),
                    new GetSensorWeightsCommand(_packetDescription)
                };
                await SendRobotCommand(commands);
            }
            else
            {
                
            }
            
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

        private async void  StartControlLoop()
        {
            while (true)
            {
                if (_rs232 != null && _rs232.IsOpen && _isManualControl)
                {
                    var enginesFill = _robotControl.EnginesFill;
                    _robotCommand = new SetEnginesCommand(enginesFill.LeftEngineFill, enginesFill.RightEngineFill, _packetDescription);
                    _robotCommand.Execute(_rs232);
                }

                await Task.Delay(10);
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
