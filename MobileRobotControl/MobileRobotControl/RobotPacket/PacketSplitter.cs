using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MobileRobotControl.RobotPacket
{
    class PacketSplitter
    {
        public delegate void PacketReceivedDelegate(string data);
        public PacketReceivedDelegate PacketReceived;
        
        private RS232 rs232Connection;
        private string packetPart = string.Empty;
        List<string> validPackets = new List<string>();

        public PacketSplitter(RS232 connection)
        {
            rs232Connection = connection;
            rs232Connection.DataReceived += rs232DataReceived;
        }

        private void rs232DataReceived(string data)
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
                if (null != PacketReceived)
                {
                    PacketReceived(p.Replace("P","").Replace("\r", ""));
                }
            }

            validPackets.Clear();
        }
    }
}
