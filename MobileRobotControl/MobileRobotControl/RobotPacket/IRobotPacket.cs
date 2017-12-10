namespace MobileRobotControl.RobotPacket
{
    public interface IRobotPacket
    {
        PacketHeaders Header { get; }
        byte[] Content { get; }
    }
}
