using System;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetEnginesCommand : IRobotCommand
    {
        private char lE, rE, dlE, drE;
        public string Content { get; private set; }

        public SetEnginesCommand(int lEngine, int rEngine, IPacketDescription packetDescription)
        {
            if (lEngine < 0)
                dlE = (char) 1;
            if (rEngine < 0)
                drE = (char) 1;

            lEngine = Math.Abs(lEngine);
            rEngine = Math.Abs(rEngine);

            if (lEngine > 100 || rEngine > 100)
            {
                throw new ArgumentOutOfRangeException("Engines absolute value larger than 100");
            }
            
            lE = (char) lEngine;
            rE = (char) rEngine;

            Content = packetDescription.PacketStart + "E" + dlE +
                      lE + drE + rE + packetDescription.PacketEnd;
        }

        public void Execute(IConnector connection)
        {
            connection.Send(Content);
        }
    }
}
