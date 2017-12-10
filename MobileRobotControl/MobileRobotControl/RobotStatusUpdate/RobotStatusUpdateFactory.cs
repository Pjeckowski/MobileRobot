using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileRobotControl.RobotPacket;

namespace MobileRobotControl.RobotStatusUpdate
{
    public class RobotStatusUpdateFactory
    {
        public IRobotStatusUpdate GetRobotStatusUpdate(IRobotPacket packet)
        {
            switch (packet.Header)
            {
                case PacketHeaders.AUpdate:
                {
                    return new AngleUpdate(packet.Content);
                }
                case PacketHeaders.EFUpdate:
                {
                    return new EngineFillUpdate(packet.Content);
                }
                case PacketHeaders.XUpdate:
                {
                    return new XPosUpdate(packet.Content);
                }
                case  PacketHeaders.YUpdate:
                {
                    return new YPosUpdate(packet.Content);
                }
                default:
                {
                    throw new ArgumentException("Unrecognized packet header.");
                }
            }
        }
    }
}
