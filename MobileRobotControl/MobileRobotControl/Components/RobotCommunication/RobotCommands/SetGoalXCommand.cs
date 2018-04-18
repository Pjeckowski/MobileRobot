using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalXCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetGoalXCommand(double goalX, IPacketDescription packetDescription)
        {
            int x =(int) (goalX * 100);

            Content = packetDescription.PacketStart + "X" +
            goalX + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
