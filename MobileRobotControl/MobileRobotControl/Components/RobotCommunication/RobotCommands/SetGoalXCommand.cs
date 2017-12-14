namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalXCommand : IRobotCommand
    {
        private float x;
        public string Content { get; private set; }
    }
}
