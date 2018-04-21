using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class MiddleWeightUpdate : IRobotStatusUpdate
    {
        private readonly double _mw;

        public MiddleWeightUpdate(string content)
        {
            _mw = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateMiddleWeight(_mw);
        }
    }
}
