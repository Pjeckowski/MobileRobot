using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class RobotStopCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public RobotStopCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "S" +
                      packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
