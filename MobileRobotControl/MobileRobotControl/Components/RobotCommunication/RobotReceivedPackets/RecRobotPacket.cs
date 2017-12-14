using System;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets
{
    public class RecRobotPacket: IRecRobotPacket
    {
        public RecPacketHeaders Header { get; private set; }
        public string Content { get; private set; }

        public RecRobotPacket(string receivedData)
        {
            if (Enum.IsDefined(typeof(RecPacketHeaders), (int) receivedData[0]))
            {
                Header = (RecPacketHeaders) receivedData[0];
                Content = receivedData.Replace(receivedData[0].ToString(), "");
            }
            else
            {
                throw new ArgumentException("Unrecognized packet header");
            }

        }
    }
}
