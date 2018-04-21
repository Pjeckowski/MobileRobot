using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class FollowerTpUpdate: IRobotStatusUpdate
    {
        private readonly int _tp;

        public FollowerTpUpdate(string content)
        {
            _tp = Convert.ToInt32(Convert.ToDouble(content) / 10000.0);
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateFollowerTp(_tp);
        }
    }
}
