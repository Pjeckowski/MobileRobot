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
        private IConnector _connection;
        private IPacketDescription _packetDescription;

        private string _packetPart = string.Empty;
        private List<string> _validPackets = new List<string>();

        public RecPacketSplitter(IPacketDescription packetDescription, IConnector connection)
        {
            _connection = connection;
            _connection.DataReceivedEvent += OnConnectionDataReceived;
            _packetDescription = packetDescription;
        }

        private void OnConnectionDataReceived(object sender, string data)
        {
            data = data.Replace(_packetDescription.PacketStart, ":" + _packetDescription.PacketStart);
            string[] packetList = Regex.Split(data, ":");
            int lastElementIndex = packetList.Length - 1;

            if (!packetList[0].Contains(_packetDescription.PacketStart) && !_packetPart.Equals(string.Empty))
            {
                packetList[0] = _packetPart + packetList[0];
                _packetPart = string.Empty;
            }

            if (!packetList[lastElementIndex].Contains(_packetDescription.PacketEnd) && packetList[lastElementIndex].Contains(_packetDescription.PacketStart))
            {
                _packetPart = packetList[lastElementIndex];
            }

            foreach (var s in packetList)
            {
                if (s.Contains(_packetDescription.PacketStart) && s.Contains(_packetDescription.PacketEnd))
                {
                    _validPackets.Add(s);
                }
            }

            foreach (var p in _validPackets)
            {
                if (null != PacketReceivedEvent)
                {
                    PacketReceivedEvent(this, p.Replace(_packetDescription.PacketStart,"").Replace(_packetDescription.PacketEnd, ""));
                }
            }

            _validPackets.Clear();
        }

      
    }
}
