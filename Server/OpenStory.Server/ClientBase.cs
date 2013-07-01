﻿using System;
using System.Timers;
using OpenStory.Common;
using OpenStory.Common.IO;
using OpenStory.Server.Fluent;
using OpenStory.Server.Processing;

namespace OpenStory.Server
{
    /// <summary>
    /// Represents a base class for all server clients.
    /// This class is abstract.
    /// </summary>
    public abstract class ClientBase : IDisposable
    {
        /// <summary>
        /// The number of pings a client is allowed to miss before being disconnected.
        /// </summary>
        private const int MissedPingsAllowed = 3;

        /// <summary>
        /// The period between pings, in milliseconds.
        /// </summary>
        private const int PingInterval = 15000;

        private bool isDisposed;

        /// <summary>
        /// Gets the client's session object.
        /// </summary>
        protected IServerSession Session { get; private set; }

        /// <summary>
        /// Gets the server which is handling this client.
        /// </summary>
        protected IGameServer Server { get; private set; }

        /// <summary>
        /// Gets the remote address for this session.
        /// </summary>
        public string RemoteAddress { get; private set; }

        /// <summary>
        /// Gets or sets the account session object.
        /// </summary>
        /// <remarks>
        /// This object is null if the client has not logged in.
        /// </remarks>
        public IAccountSession AccountSession { get; protected set; }

        private readonly Timer keepAliveTimer;
        private readonly AtomicInteger sentPings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase"/> class.
        /// </summary>
        /// <param name="server">The server that handles this client.</param>
        /// <param name="session">The session object for this client.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="server"/> or <paramref name="session"/> is <c>null</c>.
        /// </exception>
        protected ClientBase(IGameServer server, IServerSession session)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.isDisposed = false;

            this.Session = session;
            this.Session.PacketProcessing += this.OnPacketProcessing;

            this.AccountSession = null;

            this.keepAliveTimer = new Timer(PingInterval);
            this.keepAliveTimer.Elapsed += this.HandlePing;

            this.sentPings = new AtomicInteger(0);
            this.keepAliveTimer.Start();
        }

        #region Packet handling

        private void HandlePing(object sender, ElapsedEventArgs e)
        {
            OS.Log().Info("PING {0}", this.sentPings.Value);
            if (this.sentPings.Increment() > MissedPingsAllowed)
            {
                this.Disconnect("No ping response.");
                return;
            }

            using (var ping = this.Server.NewPacket("Ping"))
            {
                this.Session.WritePacket(ping.ToByteArray());
            }
        }

        private void OnPacketProcessing(object sender, PacketProcessingEventArgs e)
        {
            if (e.Label == "Pong")
            {
                var session = this.AccountSession;
                if (session != null)
                {
                    TimeSpan lag;
                    if (!session.TryKeepAlive(out lag))
                    {
                        this.Disconnect("Session keep-alive failed.");
                        return;
                    }
                }

                this.sentPings.ExchangeWith(0);
            }
            else
            {
                try
                {
                    this.ProcessPacket(e);
                }
                catch (IllegalPacketException)
                {
                    // TODO: Use IllegalPacketException for penalizing naughty clients.

                    OS.Log().Info("Received illegal packet. Client disconnected.");
                    this.Disconnect("Illegal packet.");
                }
                catch (PacketReadingException)
                {
                    OS.Log().Info("Received incomplete packet. Client disconnected.");
                    this.Disconnect("Incomplete packet.");
                }
            }
        }

        /// <summary>
        /// When implemented in a derived class, processes the provided packet data.
        /// </summary>
        /// <param name="args">The packet to be processed.</param>
        protected abstract void ProcessPacket(PacketProcessingEventArgs args);

        #endregion

        /// <summary>
        /// Writes a packet to the client's stream.
        /// </summary>
        /// <param name="data">The data of the packet.</param>
        public void WritePacket(byte[] data)
        {
            this.Session.WritePacket(data);
        }

        /// <summary>
        /// Immediately disconnects the client from the server.
        /// </summary>
        /// <param name="reason">The reason for the disconnection.</param>
        public void Disconnect(string reason = null)
        {
            LogDisconnectReason(this.AccountSession, reason);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called when the object is being freed from usage.
        /// </summary>
        /// <remarks>
        /// When overriding in a derived class, please call the base implementation after your code.
        /// </remarks>
        /// <param name="disposing">Whether the method is being called during disposal or finalization of the object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.isDisposed)
            {
                var account = this.AccountSession;
                if (account != null)
                {
                    account.Dispose();
                    this.AccountSession = null;
                }

                this.keepAliveTimer.Dispose();

                this.Session.Close();

                this.isDisposed = true;
            }
        }

        private static void LogDisconnectReason(IAccountSession session, string reason)
        {
            var reasonString = String.IsNullOrWhiteSpace(reason) ? "(no reason supplied)" : reason;

            if (session != null)
            {
                OS.Log().Info("Account session #{0} was closed: {1}", session.SessionId, reasonString);
            }
            else
            {
                OS.Log().Info("Session was closed: {0}", reasonString);
            }
        }
    }
}
