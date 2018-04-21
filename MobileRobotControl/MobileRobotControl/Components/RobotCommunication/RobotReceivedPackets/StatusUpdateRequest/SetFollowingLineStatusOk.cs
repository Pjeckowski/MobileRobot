using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class SetFollowingLineStatusOk : IRobotStatusUpdate
    {

        public SetFollowingLineStatusOk()
        {
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.SetFollowingLineOK();
        }
    }
}
