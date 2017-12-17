namespace MobileRobotControl.Components.RobotCommunication.PacketDescriber
{
    public interface IPacketDescription
    {
        string PacketStart { get; }
        string PacketEnd { get; }
    }
}
