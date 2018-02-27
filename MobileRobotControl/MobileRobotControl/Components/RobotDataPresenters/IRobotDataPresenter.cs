namespace MobileRobotControl.Components.RobotDataPresenters
{
    public interface IRobotDataPresenter
    {
        void UpdateX(float xValue);
        void UpdateY(float yValue);
        void UpdateAngle(float aValue);
        void UpdateEngines(int lEValue, int rEValue);
    }
}
