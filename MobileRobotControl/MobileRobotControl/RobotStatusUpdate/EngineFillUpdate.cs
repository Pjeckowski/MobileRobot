using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotStatusUpdate
{
    public class EngineFillUpdate : IRobotStatusUpdate
    {
        private int lEngine, rEngine;

        public EngineFillUpdate(string content)
        {
            string[] temp = content.Split(',');
            lEngine = Convert.ToInt32(temp[0]);
            rEngine = Convert.ToInt32(temp[1]);
        }

        public void Execute(MainWindow mainWindow)
        {
            mainWindow.UpdateEngines(lEngine,rEngine);
        }
    }
}
