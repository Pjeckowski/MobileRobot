using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class EngineFillUpdate : IRobotStatusUpdate
    {
        private readonly int _lEngine, _rEngine;

        public EngineFillUpdate(string content)
        {
            string[] temp = content.Split(',');
            _lEngine = Convert.ToInt32(temp[0]);
            _rEngine = Convert.ToInt32(temp[1]);
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            robotDataPresenter.UpdateEngines(_lEngine,_rEngine);
        }
    }
}
