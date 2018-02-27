using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class LineFollowCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public LineFollowCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "L" +
                        packetDescription.PacketEnd;
        }
        
        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
