namespace MobileRobotControl.Components.RobotCommunication.RobotCommands
{
    public class SetGoalYCommand : IRobotCommand
    {
        private float y;
        public string Content { get; private set; }
    }
}
