using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalXCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetGoalXCommand(float goalX, IPacketDescription packetDescription)
        {
            int x =(int) (goalX * 100);

            while (0 != x)
            {
                Content += (x % 10).ToString();
                x /= 10;
            }

            char[] contentCharArray = Content.ToCharArray();
            Array.Reverse(contentCharArray);
            Content = packetDescription.PacketStart + "X" +
            new string(contentCharArray) + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
