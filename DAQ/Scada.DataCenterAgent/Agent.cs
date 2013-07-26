﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private TcpClient client;

        private NetworkStream stream;

        private DataHandler handler;

        private bool connected = false;

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

        public Agent()
        {
            this.handler = new DataHandler(this);
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
            return this.ServerAddress;
        }

        public void Connect()
        {
            if ((this.client == null) || (!this.client.Connected))
            {
                try
                {
                    this.client = new TcpClient();
                    this.client.ReceiveTimeout = 10000;

                    this.client.BeginConnect(this.ServerAddress, this.ServerPort, new AsyncCallback(ConnectCallback), this.client);
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

                        // [Auth]
                        this.handler.SendAuthPacket();

                        this.BeginRead(this.client);
                    }
                }
                catch (SocketException)
                {
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
            this.stream.Write(message, 0, message.Length);
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
    }
}
