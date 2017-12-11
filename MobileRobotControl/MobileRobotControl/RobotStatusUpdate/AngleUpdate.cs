using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotStatusUpdate
{
    public class AngleUpdate: IRobotStatusUpdate
    {
        private float angle;

        public AngleUpdate(string content)
        {
            angle = Convert.ToSingle(content) / 100;
        }

        public void Execute(MainWindow mainWindow)
        {
            mainWindow.UpdateAngle(angle);
        }
    }
}
