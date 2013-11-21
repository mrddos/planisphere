﻿using Newtonsoft.Json.Linq;
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
        private const string Post = @"POST";

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
            /*
            this.wc.DownloadStringCompleted += DownloadStringCompleted;
            this.wc.UploadFileCompleted += UploadFileCompleted;
            this.wc.UploadDataCompleted += UploadDataCompleted;
            */
        }


        /*
        */

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

        private string GetUrl(string api)
        {
            return string.Format("http://{0}:{1}/{2}", this.ServerAddress, this.ServerPort, api);
        }

        internal void SendDataPacket(Packet packet, DateTime time)
        {
            this.Send("data/commit", packet, time);
        }

        private void Send(string api, Packet packet, DateTime time)
        {
            try
            {
                Uri uri = new Uri(this.GetUrl(api));
                byte[] data = Encoding.ASCII.GetBytes(packet.ToString());
                WebClient wc = new WebClient();
                wc.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) => 
                    {
                        if (e.Error != null)
                        {
                            return;
                        }
                        Packet p = (Packet)e.UserState;
                        if (p != null)
                        {
                            string result = Encoding.ASCII.GetString(e.Result);
                            // TODO: with result
                        }

                    };
                wc.UploadDataAsync(uri, Post, data, packet);
                
            }
            catch (Exception e)
            {

            }
        }

        internal void FetchCommands()
        {
            Uri uri = new Uri(this.GetUrl("cmd/query"));
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                    {
                        if (e.Error == null)
                        {
                            this.ParseCommand(e.Result);
                        }

                    };
                wc.DownloadStringAsync(uri);
            }
            catch (Exception)
            {
            }
        }

        private void ParseCommand(string cmd)
        {
            try
            {
                JObject json = JObject.Parse(cmd);

            }
            catch (Exception)
            {
            }
        }

        internal void SendPacket(Packet p)
        {
            this.SendDataPacket(p, default(DateTime));
        }

        internal void SendFilePacket(Packet packet)
        {
            Uri uri = new Uri(this.GetUrl("data/upload"));
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.UploadFileCompleted += (object sender, UploadFileCompletedEventArgs e) =>
                        {
                            if (e.Error != null)
                            {
                                return;
                            }
                            Packet p = (Packet)e.UserState;
                            if (p != null)
                            {
                                // TODO: with p.Path
                            }
                        };
                    wc.UploadFileAsync(uri, Post, packet.Path, packet);
                }
            }
            catch (WebException)
            {
             
            }
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
