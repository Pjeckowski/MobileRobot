using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class GetBaseParametersCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public GetBaseParametersCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "B" +
                        packetDescription.PacketEnd;
        }
        
        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
