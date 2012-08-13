﻿using OpenStory.Common;
using OpenStory.Common.Authentication;

namespace OpenStory.Server.Authentication
{
    internal class ChannelInfo : IChannel
    {
        private readonly AtomicInteger channelLoad;

        public ChannelInfo()
        {
            this.channelLoad = 0;
        }

        #region IChannel Members

        public byte Id { get; private set; }
        public byte WorldId { get; private set; }
        public string Name { get; private set; }

        public int ChannelLoad
        {
            get { return this.channelLoad.Value; }
        }

        #endregion
    }
}