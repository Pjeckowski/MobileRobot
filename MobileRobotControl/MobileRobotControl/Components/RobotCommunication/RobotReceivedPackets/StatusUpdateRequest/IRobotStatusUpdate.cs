using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public interface IRobotStatusUpdate
    {
        void Execute(IRobotDataPresenter robotDataPresenter);
    }
}
