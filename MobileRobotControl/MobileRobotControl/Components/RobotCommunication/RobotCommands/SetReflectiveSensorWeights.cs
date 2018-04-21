using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetReflectiveSensorWeights : IRobotCommand
    {
        public string Content { get; private set; }

        public SetReflectiveSensorWeights(double nearestSensor, double middleSensor, double farthestSensor, IPacketDescription packetDescription)
        {
            int nSensor = Convert.ToInt32(nearestSensor * 1000);
            int mSensor =Convert.ToInt32(middleSensor * 1000);
            int fSensor =Convert.ToInt32(farthestSensor * 1000);

            Content = packetDescription.PacketStart + "W" + "N" + 
                nSensor + "M" + mSensor + "F" + fSensor + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
