namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets
{
    public enum RecPacketHeaders
    {
        XUpdate = 'X',
        YUpdate = 'Y',
        AUpdate = 'A',
        EFUpdate = 'E',
        WSizeUpdate = 'W',
        WSpacingUpdate = 'S',
        WeightNearestUpdate = 'N',
        WeightMiddleUpdate = 'M',
        WeightFahrtestUpdate = 'F',
        FollowerKPUpdate = 'K',
        FollowerTPUpdate = 'T',
        FollowingPointOKUpdate = 'R',
        FollowingLineOKUpdate = 'L'
    }
}
