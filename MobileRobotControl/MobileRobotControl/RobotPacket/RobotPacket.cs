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
        private byte[] content = new byte[4];
        public byte[] Content 
        {
            get { return content; }
        }

        public RobotPacket(string receivedData)
        {
            byte[] toBytes = Encoding.ASCII.GetBytes(receivedData);
            if (Enum.IsDefined(typeof(PacketHeaders),(int) toBytes[0]))
            {
                Header = (PacketHeaders) toBytes[0];

                for (int i = 0; i < content.Length; i++)
                {
                    content[i] = toBytes[i + 1];
                }
            }
            else
            {
                throw new ArgumentException("Unrecognized packet header");
            }

        }
    }
}
