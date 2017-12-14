using System;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
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
