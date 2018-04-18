namespace MobileRobotControl.Components.RobotDataPresenters
{
    public interface IRobotDataPresenter
    {
        void UpdateX(double x);
        void UpdateY(double y);
        void UpdateAngle(double angle);
        void UpdateWheelSize(double wSize);
        void UpdateWheelSpacing(double wSpacing);
        void UpdateEngines(int lEValue, int rEValue);
    }
}
