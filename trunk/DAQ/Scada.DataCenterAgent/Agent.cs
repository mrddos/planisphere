using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Scada.DataCenterAgent
{
    internal class StateObject
    {
        public const int BufferSize = 1024;

        public TcpClient client = null;

        public int totalBytesRead = 0;


        public string readType = null;

        public byte[] buffer = new byte[BufferSize];


        public StringBuilder messageBuffer = new StringBuilder();
    }

    internal delegate void OnReceiveMessage(Agent agent, string msg);


    class Agent
    {
        // Wired connection Tcp client
        private TcpClient client = null;

        // Wireless connection Tcp client
        private TcpClient wirelessClient = null;

        // Maybe from wired, or wireless.
        private NetworkStream stream;

        // the current data handler.
        private DataHandler handler;

        

        internal bool Started
        {
            get;
            set;
        }

        internal bool History
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

        public string WirelessServerAddress
        {
            get;
            set;
        }

        public int WirelessServerPort
        {
            set;
            get;
        }

        // Hello message.
        public string Greeting
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public bool Wireless
        {
            get;
            set;
        }

        public OnReceiveMessage OnReceiveMessage
        {
            get;
            set;
        }

        public override string ToString()
        {
            return (this.client != null) 
                ? string.Format("{0}:{1}", this.ServerAddress, this.ServerPort) 
                : string.Format("{0}:{1}", this.WirelessServerAddress, this.WirelessServerPort);
        }

        public void Connect()
        {
            if ((this.client == null) || (!this.client.Connected))
            {
                try
                {
                    this.client = new TcpClient();
                    this.client.ReceiveTimeout = 10000;

                    this.client.BeginConnect(
                        this.ServerAddress, this.ServerPort, 
                        new AsyncCallback(ConnectCallback), 
                        this.client);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void ConnectToWireless()
        {
            if ((this.wirelessClient == null) || (!this.wirelessClient.Connected))
            {
                try
                {
                    this.wirelessClient = new TcpClient();
                    this.wirelessClient.ReceiveTimeout = 10000;

                    this.wirelessClient.BeginConnect(
                        this.WirelessServerAddress, this.WirelessServerPort, 
                        new AsyncCallback(ConnectToWirelessCallback), 
                        this.wirelessClient);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    TcpClient client = (TcpClient)result.AsyncState;
                    
                    client.EndConnect(result);
                    //client.
                    if (client.Connected)
                    {
                        this.stream = this.client.GetStream();
                        this.BeginRead(this.client);

                        this.handler = new DataHandler(this);
                        // [Auth]
                        this.handler.SendAuthPacket();
                    }
                }
                catch (SocketException e)
                {
                    this.client = null;
                    var s = e.Message;
                    this.ConnectToWireless();
                }

            }
        }

        private void ConnectToWirelessCallback(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    TcpClient client = (TcpClient)result.AsyncState;
                    client.EndConnect(result);
                    //client.
                    if (client.Connected)
                    {
                        this.stream = this.client.GetStream();
                        this.BeginRead(this.client);

                        this.handler = new DataHandler(this);
                        // [Auth]
                        this.handler.SendAuthPacket();
                    }
                }
                catch (SocketException e)
                {
                    this.wirelessClient = null;
                    var s = e.Message;
                }

            }
        }


        private void BeginRead(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            if (stream.CanRead)
            {
                try
                {
                    StateObject so = new StateObject() { client = client };
                    IAsyncResult ar = stream.BeginRead(so.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), so);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                StateObject so = (StateObject)result.AsyncState;
                try
                {
                    NetworkStream stream = client.GetStream();
                    int c = stream.EndRead(result);

                    if (c > 0)
                    {
                        string msg = Encoding.ASCII.GetString(so.buffer, 0, c);
                        this.OnReceivedMessagess(msg);
                        this.BeginRead(so.client);
                    }
                }
                catch (Exception e)
                {
                    string readErrorMessage = e.Message;
                }
            }
        }

        private void OnReceivedMessagess(string messages)
        {
            string[] msgs = messages.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string msg in msgs)
            {
                if (msg.Trim() != string.Empty)
                {
                    this.OnReceiveMessage(this, msg);
                    if (this.handler != null)
                    {
                        this.handler.OnMessage(msg);
                    }
                }
            }
        }


        private void Send(byte[] message)
        {
            try
            {
                if (this.stream != null)
                {
                    this.stream.Write(message, 0, message.Length);
                }
            }
            catch(IOException e)
            {
                this.stream = null;
                if (this.client != null)
                {
                    this.client = null;
                    this.ConnectToWireless();
                }
            }
        }

        internal void SendPacket(DataPacket p, DateTime time)
        {
            string s = p.ToString();
            this.Send(Encoding.ASCII.GetBytes(s));
        }

        internal void SendPacket(DataPacket p)
        {
            this.SendPacket(p, default(DateTime));
        }

        internal void SendDataPacket(DataPacket p, DateTime time)
        {
            if (p == null)
                return;
            // Only start or history.
            if (this.Started || this.History)
            {
                string s = p.ToString();
                this.Send(Encoding.ASCII.GetBytes(s));
            }
        }


        internal void SendReplyPacket(DataPacket p, DateTime time)
        {
            string s = p.ToString();
            this.Send(Encoding.ASCII.GetBytes(s));
        }

        internal void AddWirelessInfo(string wirelessServerAddress, int wirelessServerPort)
        {
            this.WirelessServerAddress = wirelessServerAddress;
            this.WirelessServerPort = wirelessServerPort;
        }
    }
}
