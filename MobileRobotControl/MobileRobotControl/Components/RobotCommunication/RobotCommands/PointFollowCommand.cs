using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriber;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class PointFollowCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public PointFollowCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "P" +
          packetDescription.PacketEnd;
        }
        
        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
