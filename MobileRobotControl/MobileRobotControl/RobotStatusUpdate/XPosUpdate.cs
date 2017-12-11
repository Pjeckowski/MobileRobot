using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotStatusUpdate
{
    public class XPosUpdate : IRobotStatusUpdate
    {
        private float x;

        public XPosUpdate(string content)
        {
            x = Convert.ToSingle(content) / 100;
        }

        public void Execute(MainWindow mainWindow)
        {
            mainWindow.UpdateX(x);
        }
    }
}
