namespace MobileRobotControl.Components.RobotDataPresenters
{
    public interface IRobotDataPresenter
    {
        void UpdateX(double x);
        void UpdateY(double y);
        void UpdateAngle(double angle);
        void UpdateWheelSize(double wSize);
        void UpdateWheelSpacing(double wSpacing);
        void UpdateNearestWeight(double nWeight);
        void UpdateMiddleWeight(double mWeight);
        void UpdateFahrtestWeight(double fWeight);
        void UpdateFollowerKp(double kp);
        void UpdateFollowerTp(int tp);
        void UpdateEngines(int lEValue, int rEValue);
        void SetFollowingLineOK();
        void SetFollowingPointOK();
    }
}
