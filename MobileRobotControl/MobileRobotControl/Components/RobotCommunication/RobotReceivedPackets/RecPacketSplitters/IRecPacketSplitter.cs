using System;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.RecPacketSplitters
{
    public interface IRecPacketSplitter
    {
        event EventHandler<string> PacketReceivedEvent;
    }
}
