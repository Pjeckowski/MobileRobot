using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalYCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetGoalYCommand(double goalY, IPacketDescription packetDescription)
        {
            int y =(int) (goalY * 100);

            Content = packetDescription.PacketStart + "Y" +
                goalY + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
