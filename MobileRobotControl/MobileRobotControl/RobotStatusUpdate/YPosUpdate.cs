using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotStatusUpdate
{
    public class YPosUpdate : IRobotStatusUpdate
    {
        private float y;

        public YPosUpdate(string content)
        {
            y = Convert.ToSingle(content) / 100;
        }

        public void Execute(MainWindow mainWindow)
        {
            mainWindow.UpdateY(y);
        }
    }
}
