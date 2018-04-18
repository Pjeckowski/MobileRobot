using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class WSpacingUpdate : IRobotStatusUpdate
    {
        private readonly double _wheelSpacing;

        public WSpacingUpdate(string content)
        {
            _wheelSpacing = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateWheelSpacing(_wheelSpacing);
        }
    }
}
