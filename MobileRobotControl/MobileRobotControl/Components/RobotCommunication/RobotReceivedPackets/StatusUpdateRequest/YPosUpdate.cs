using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class YPosUpdate : IRobotStatusUpdate
    {
        private readonly double _y;

        public YPosUpdate(string content)
        {
            _y = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateY(_y);
        }
    }
}
