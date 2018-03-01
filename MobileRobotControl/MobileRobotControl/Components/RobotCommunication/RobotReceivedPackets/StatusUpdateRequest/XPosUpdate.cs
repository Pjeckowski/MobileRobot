using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class XPosUpdate : IRobotStatusUpdate
    {
        private readonly float _x;

        public XPosUpdate(string content)
        {
            _x = Convert.ToSingle(content) / 100;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateX(_x);
        }
    }
}
