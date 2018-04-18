﻿using System;
using MobileRobotControl.Components.RobotDataPresenters;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class AngleUpdate: IRobotStatusUpdate
    {
        private readonly double _angle;

        public AngleUpdate(string content)
        {
            _angle = Convert.ToDouble(content) / 10000.0;
        }

        public void Execute(IRobotDataPresenter robotDataPresenter)
        {
            var degrees = _angle * 180.0 / Math.PI;
            robotDataPresenter.UpdateAngle(degrees);
        }
    }
}
