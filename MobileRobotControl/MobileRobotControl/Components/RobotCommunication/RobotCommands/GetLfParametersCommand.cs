using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class GetLfParametersCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public GetLfParametersCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "F" +
                        packetDescription.PacketEnd;
        }
        
        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
