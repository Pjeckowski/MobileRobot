using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MobileRobotControl.Components.Connection;
using MobileRobotControl.Components.RobotCommunication.PacketDescriptions;

namespace MobileRobotControl.Components.RobotCommunication.RobotReceivedPackets.RecPacketSplitters
{
    public class RecPacketSplitter : IRecPacketSplitter
    {
        public event EventHandler<string> PacketReceivedEvent;
        private IConnector connection;
        private IPacketDescription packetDescription;

        private string packetPart = string.Empty;
        private List<string> validPackets = new List<string>();

        public RecPacketSplitter(IPacketDescription packetDescription, IConnector connection)
        {
            this.connection = connection;
            this.connection.DataReceivedEvent += ConnectionDataReceived;
            this.packetDescription = packetDescription;
        }

        private void ConnectionDataReceived(object sender, string data)
        {
            data = data.Replace(packetDescription.PacketStart, ":" + packetDescription.PacketStart);
            string[] packetList = Regex.Split(data, ":");
            int lenght = packetList.Length;

            if (!packetList[0].Contains(packetDescription.PacketStart) && !packetPart.Equals(string.Empty))
            {
                packetList[0] = packetPart + packetList[0];
                packetPart = string.Empty;
            }

            if (!packetList[lenght - 1].Contains(packetDescription.PacketEnd) && packetList[lenght - 1].Contains(packetDescription.PacketStart))
            {
                packetPart = packetList[lenght - 1];
            }

            foreach (var s in packetList)
            {
                if (s.Contains(packetDescription.PacketStart) && s.Contains(packetDescription.PacketEnd))
                {
                    validPackets.Add(s);
                }
            }

            foreach (var p in validPackets)
            {
                if (null != PacketReceivedEvent)
                {
                    PacketReceivedEvent(this, p.Replace(packetDescription.PacketStart,"").Replace(packetDescription.PacketEnd, ""));
                }
            }

            validPackets.Clear();
        }

      
    }
}
