using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MobileRobotControl.RobotStatusUpdate
{
    public interface IRobotStatusUpdate
    {
        void Execute(MainWindow mainWindow);
    }
}
