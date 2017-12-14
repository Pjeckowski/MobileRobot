using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public interface IRobotCommand 
    {
        string Content { get; }
    }
}
