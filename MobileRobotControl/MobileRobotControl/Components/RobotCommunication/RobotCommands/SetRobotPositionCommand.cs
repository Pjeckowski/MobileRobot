using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetRobotPositionCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetRobotPositionCommand(double x, double y, double angle, IPacketDescription packetDescription)
        {
            int vx = Convert.ToInt32(x * 1000);
            int vy = Convert.ToInt32(y * 1000);
            int vangle = Convert.ToInt32(angle * Math.PI / 180.0 * 100000.0);

            Content = packetDescription.PacketStart + "N" + "X" + 
                vx + "Y" + vy + "A" + vangle + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
