using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Scada.Data.Client
{
    /// <summary>
    /// 
    /// </summary>
    public enum NotifyEvent
    {
        Connected,
        ConnectError,
        ConnectToCountryCenter,
        DisconnectToCountryCenter,
    }

    public enum Type
    {
        Province = 1,
        Country = 2,
    }

    public delegate void OnReceiveMessage(Agent agent, string msg);

    public delegate void OnNotifyEvent(Agent agent, NotifyEvent ne, string msg);

    /// <summary>
    /// 
    /// </summary>
    public class Agent
    {
        private WebClient wc = new WebClient();

        private const int Timeout = 5000;
        
        internal bool SendDataStarted
        {
            get;
            set;
        }

        internal bool SendDataDirectlyStarted
        {
            get;
            set;
        }

        internal bool OnHistoryData
        {
            get;
            set;
        }

        public Agent(string serverAddress, int serverPort)
        {
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
        }

        public string ServerAddress
        {
            get;
            set;
        }

        public int ServerPort
        {
            set;
            get;
        }

        public override string ToString()
        {
            return "";
           
        }
        internal void SendPacket(Packet p, DateTime time)
        {
            string s = p.ToString();
            // this.Send(Encoding.ASCII.GetBytes(s));
        }

        internal void SendPacket(Packet p)
        {
            this.SendPacket(p, default(DateTime));
        }
        
        internal void SendReplyPacket(Packet p, DateTime time)
        {
            string s = p.ToString();
            // this.Send(Encoding.ASCII.GetBytes(s));
        }

        // Connect means first HTTP packet to the data Center.
        internal void Connect()
        {
            // TODO: Send Packet of init.

        }
    }
}
