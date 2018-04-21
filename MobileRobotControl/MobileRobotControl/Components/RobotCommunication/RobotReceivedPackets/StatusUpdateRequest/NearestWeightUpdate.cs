using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class NearestWeightUpdate : IRobotStatusUpdate
    {
        private readonly double _nw;

        public NearestWeightUpdate(string content)
        {
            _nw = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateNearestWeight(_nw);
        }
    }
}
