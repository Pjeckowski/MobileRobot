using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class GetSensorWeightsCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public GetSensorWeightsCommand(IPacketDescription packetDescription)
        {
            Content = packetDescription.PacketStart + "D" +
                        packetDescription.PacketEnd;
        }
        
        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
