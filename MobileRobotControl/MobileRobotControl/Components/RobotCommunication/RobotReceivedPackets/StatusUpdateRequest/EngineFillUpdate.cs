using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class EngineFillUpdate : IRobotStatusUpdate
    {
        private int lEngine, rEngine;

        public EngineFillUpdate(string content)
        {
            string[] temp = content.Split(',');
            lEngine = Convert.ToInt32(temp[0]);
            rEngine = Convert.ToInt32(temp[1]);
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateEngines(lEngine,rEngine);
        }
    }
}
