using System;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.StatusUpdateRequest
{
    public class RobotStatusUpdateFactory
    {
        public IRobotStatusUpdate GetRobotStatusUpdate(IRecRobotPacket packet)
        {
            switch (packet.Header)
            {
                case RecPacketHeaders.AUpdate:
                {
                    return new AngleUpdate(packet.Content);
                }
                case RecPacketHeaders.EFUpdate:
                {
                    return new EngineFillUpdate(packet.Content);
                }
                case RecPacketHeaders.XUpdate:
                {
                    return new XPosUpdate(packet.Content);
                }
                case RecPacketHeaders.YUpdate:
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
