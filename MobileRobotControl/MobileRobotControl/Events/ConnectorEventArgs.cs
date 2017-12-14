using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.Events
{
    public class ConnectorEventArgs : EventArgs
    {
        string Data;

        public ConnectorEventArgs(string data)
        {
            Data = data;
        }
    }
}
