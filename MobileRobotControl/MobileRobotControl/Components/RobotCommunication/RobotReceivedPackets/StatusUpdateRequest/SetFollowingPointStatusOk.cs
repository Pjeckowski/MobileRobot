using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class SetFollowingPointStatusOk : IRobotStatusUpdate
    {

        public SetFollowingPointStatusOk()
        {
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.SetFollowingLineOK();
        }
    }
}
