using System;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
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
