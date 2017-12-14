using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.RecPacketSplitter
{
    public interface IRecPacketSplitter
    {
        event EventHandler<string> PacketReceivedEvent;
    }
}
