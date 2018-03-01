using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class AngleUpdate: IRobotStatusUpdate
    {
        private readonly float _angle;

        public AngleUpdate(string content)
        {
            _angle = Convert.ToSingle(content) / 100;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateAngle(_angle);
        }
    }
}
