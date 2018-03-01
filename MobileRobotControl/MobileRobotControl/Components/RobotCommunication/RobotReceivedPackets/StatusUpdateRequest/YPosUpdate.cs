using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class YPosUpdate : IRobotStatusUpdate
    {
        private readonly float _y;

        public YPosUpdate(string content)
        {
            _y = Convert.ToSingle(content) / 100;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateY(_y);
        }
    }
}
