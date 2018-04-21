using System.Windows;

namespace MobileRobotControl.Windows
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public WarningWindow(string warningMessage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            WariningMessageLabel.Content = warningMessage;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
