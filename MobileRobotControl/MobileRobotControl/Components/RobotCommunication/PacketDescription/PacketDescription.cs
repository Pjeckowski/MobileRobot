namespace MobileRobotControl.Components.RobotCommunication.PacketDescriber
{
    public class PacketDescription : IPacketDescription
    {
        public string PacketStart { get; private set; }
        public string PacketEnd { get; private set; }

        public PacketDescription(string packetStart, string packetEnd)
        {
            PacketStart = packetStart;
            PacketEnd = packetEnd;
        }
    }
}
