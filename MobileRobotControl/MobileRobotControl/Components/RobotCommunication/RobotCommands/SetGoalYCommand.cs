using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalYCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetGoalYCommand(float goalY, IPacketDescription packetDescription)
        {
            int y =(int) (goalY * 100);

            while (0 != y)
            {
                Content = (y % 10).ToString();
                y /= 10;
            }
            char[] contentCharArray = Content.ToCharArray();
            Array.Reverse(contentCharArray);
            Content = packetDescription.PacketStart + "Y" +
                contentCharArray + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
