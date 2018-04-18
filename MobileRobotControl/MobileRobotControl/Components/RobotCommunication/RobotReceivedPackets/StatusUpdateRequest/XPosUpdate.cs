using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class XPosUpdate : IRobotStatusUpdate
    {
        private readonly double _x;

        public XPosUpdate(string content)
        {
            _x = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateX(_x);
        }
    }
}
