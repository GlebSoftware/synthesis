﻿using SynthesisMultiplayer.Threading;
using SynthesisMultiplayer.Util;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SynthesisMultiplayer.Common
{
    public class ManagedUDPTask : ManagedTask
    {
        bool disposed;
        protected IPEndPoint Endpoint;
        protected UdpClient Connection;
        public ManagedUDPTask(Channel<(string, AsyncCallHandle?)> statusChannel,
            Channel<(string, AsyncCallHandle?)> messageChannel,
            IPAddress ip,
            int port = 33000) : base()
        {
            StatusChannel = statusChannel;
            MessageChannel = messageChannel;
            Endpoint = new IPEndPoint(ip, port);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Connection.Close();
                    Connection.Dispose();
                    MessageChannel.Dispose();
                }
                disposed = true;
            }
        }
    }
}
