using MobileRobotControl.Components.Connection;

namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public interface IRobotCommand 
    {
        string Content { get; }
        void Execute(IConnector connection);
    }
}
