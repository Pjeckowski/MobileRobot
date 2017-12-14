namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets
{
    public interface IRecRobotPacket
    {
        RecPacketHeaders Header { get; }
        string Content { get; }
    }
}
