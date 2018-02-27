namespace MobileRobotControl.Components.RobotCommunication.PacketDescriptions
{
    public interface IPacketDescription
    {
        string PacketStart { get; }
        string PacketEnd { get; }
    }
}
