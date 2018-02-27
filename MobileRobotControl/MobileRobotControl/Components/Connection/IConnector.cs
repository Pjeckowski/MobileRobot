using System;

namespace MobileRobotControl.Components.Connection
{
    public interface IConnector
    {
        bool Send(string data);
        string Receive();
        event EventHandler<string> DataReceivedEvent;
    }
}
