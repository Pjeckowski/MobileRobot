using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MobileRobotControl.Components.Connection;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.RecPacketSplitter
{
    public class RecPacketSplitter : IRecPacketSplitter
    {
        public event EventHandler<string> PacketReceivedEvent;
        private IConnector connection;
        private string packetPart = string.Empty;
        List<string> validPackets = new List<string>();

        public RecPacketSplitter(IConnector connection)
        {
            this.connection = connection;
            this.connection.DataReceivedEvent += connectionDataReceived;
        }

        private void connectionDataReceived(object sender, string data)
        {
            string[] packetList = Regex.Split(data, @"(?=P)");
            int lenght = packetList.Length;

            if (!packetList[0].Contains("P") && !packetPart.Equals(string.Empty))
            {
                packetList[0] = packetPart + packetList[0];
                packetPart = string.Empty;
            }

            if (!packetList[lenght - 1].Contains("\r") && packetList[lenght - 1].Contains("P"))
            {
                packetPart = packetList[lenght - 1];
            }

            foreach (var s in packetList)
            {
                if (s.Contains("P") && s.Contains("\r"))
                {
                    validPackets.Add(s);
                }
            }

            foreach (var p in validPackets)
            {
                if (null != PacketReceivedEvent)
                {
                    PacketReceivedEvent(this, p.Replace("P","").Replace("\r", ""));
                }
            }

            validPackets.Clear();
        }

      
    }
}
