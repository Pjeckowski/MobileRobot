using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetLineFollowerParametersCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetLineFollowerParametersCommand(double kp, int tp, IPacketDescription packetDescription)
        {
            int ikp = Convert.ToInt32(kp * 1000);
            int itp =Convert.ToInt32(tp * 1000);

            Content = packetDescription.PacketStart + "Z" + "K" + 
                ikp + "T" + itp + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
