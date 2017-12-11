using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotPacket
{
    public class RobotPacket: IRobotPacket
    {
        public PacketHeaders Header { get; private set; }
        public string Content { get; private set; }

        public RobotPacket(string receivedData)
        {
            if (Enum.IsDefined(typeof(PacketHeaders), receivedData[0]))
            {
                Header = (PacketHeaders) receivedData[0];
                Content = receivedData.Replace(receivedData[0].ToString(), "");
            }
            else
            {
                throw new ArgumentException("Unrecognized packet header");
            }

        }
    }
}
