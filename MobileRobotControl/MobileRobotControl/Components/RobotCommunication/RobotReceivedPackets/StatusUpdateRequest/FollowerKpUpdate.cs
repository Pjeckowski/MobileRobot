using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class FollowerKpUpdate : IRobotStatusUpdate
    {
        private readonly double _kp;

        public FollowerKpUpdate(string content)
        {
            _kp = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateFollowerKp(_kp);
        }
    }
}
