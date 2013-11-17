﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Data.Client
{
    public class PacketBuilder
    {
        internal List<Packet> GetPackets(string deviceKey, DateTime time, string content)
        {
            throw new NotImplementedException();
        }

        internal Packet GetFlowPacket(string deviceKey, Dictionary<string, object> d, bool p)
        {
            throw new NotImplementedException();
        }

        internal Packet GetPacket(string deviceKey, Dictionary<string, object> data, bool p)
        {
            Packet packet = new Packet();
            packet.Station = "A";
            packet.Token = "";
            packet.AddData(deviceKey, data);
            return packet;
        }
    }
}
