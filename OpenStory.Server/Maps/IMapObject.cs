﻿using System.Drawing;

namespace OpenStory.Server.Maps
{
    internal interface IMapObject
    {
        int ObjectId { get; }
        MapObjectType Type { get; }
        Point Position { get; }

        void SendSpawnData(ChannelClient client);
        void SendDestroyData(ChannelClient client);
    }
}