using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class FarthestWeightUpdate : IRobotStatusUpdate
    {
        private readonly double _fw;

        public FarthestWeightUpdate(string content)
        {
            _fw = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateFahrtestWeight(_fw);
        }
    }
}
