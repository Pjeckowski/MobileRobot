using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class XPosUpdate : IRobotStatusUpdate
    {
        private float x;

        public XPosUpdate(string content)
        {
            x = Convert.ToSingle(content) / 100;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateX(x);
        }
    }
}
