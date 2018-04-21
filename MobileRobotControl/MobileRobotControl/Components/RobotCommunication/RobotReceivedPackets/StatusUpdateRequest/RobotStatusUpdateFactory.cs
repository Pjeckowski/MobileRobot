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
                case RecPacketHeaders.WSizeUpdate:
                {
                    return new WSizeUpdate(packet.Content);
                }
                case RecPacketHeaders.WSpacingUpdate:
                {
                    return new WSpacingUpdate(packet.Content);
                }
                case RecPacketHeaders.WeightFahrtestUpdate:
                {
                    return new FarthestWeightUpdate(packet.Content);
                }
                case RecPacketHeaders.WeightMiddleUpdate:
                {
                    return new MiddleWeightUpdate(packet.Content);
                }
                case RecPacketHeaders.WeightNearestUpdate:
                {
                    return new NearestWeightUpdate(packet.Content);
                }
                case RecPacketHeaders.FollowerKPUpdate:
                {
                    return new FollowerKpUpdate(packet.Content);
                }
                case RecPacketHeaders.FollowerTPUpdate:
                {
                    return new FollowerTpUpdate(packet.Content);
                }
                case RecPacketHeaders.FollowingLineOKUpdate:
                {
                    return new SetFollowingLineStatusOk();
                }
                case RecPacketHeaders.FollowingPointOKUpdate:
                {
                    return new SetFollowingPointStatusOk();
                }
                default:
                {
                    throw new ArgumentException("Unrecognized packet header.");
                }
            }
        }
    }
}
