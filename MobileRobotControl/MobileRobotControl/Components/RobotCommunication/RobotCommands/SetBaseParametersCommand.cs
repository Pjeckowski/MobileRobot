using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetBaseParametersCommand : IRobotCommand
    {
        public string Content { get; private set; }

        public SetBaseParametersCommand(double wheelSize, double wheelSpacing, IPacketDescription packetDescription)
        {
            int wSize = Convert.ToInt32(wheelSize * 1000);
            int wSpacing =Convert.ToInt32(wheelSpacing * 1000);

            Content = packetDescription.PacketStart + "C" + "W" + 
                wSize + "S" + wSpacing + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
