using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetEnginesCommand : IRobotCommand
    {

        public string Content { get; private set; }

        public SetEnginesCommand(int lEngine, int rEngine, IPacketDescription packetDescription)
        {
            char lEngineDir = (char)0;
            char rEngineDir = (char)0;

            if (lEngine < 0)
                lEngineDir = (char) 1;
            if (rEngine < 0)
                rEngineDir = (char) 1;

            lEngine = Math.Abs(lEngine);
            rEngine = Math.Abs(rEngine);

            if (lEngine > 100 || rEngine > 100)
            {
                throw new ArgumentOutOfRangeException("Engines absolute value larger than 100");
            }
            
            Content = packetDescription.PacketStart + "E" + lEngineDir +
                      (char)lEngine + rEngineDir + (char)rEngine + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
