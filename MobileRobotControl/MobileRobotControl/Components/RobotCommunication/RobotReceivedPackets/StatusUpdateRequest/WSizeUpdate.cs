using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class WSizeUpdate : IRobotStatusUpdate
    {
        private readonly double _wheelSize;

        public WSizeUpdate(string content)
        {
            _wheelSize = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateWheelSize(_wheelSize);
        }
    }
}
