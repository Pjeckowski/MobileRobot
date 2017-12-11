namespace MobileRobotControl.RobotPacket
{
    public interface IRobotPacket
    {
        PacketHeaders Header { get; }
        string Content { get; }
    }
}
