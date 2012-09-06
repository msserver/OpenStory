﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OpenStory.Server.Data
{
    /// <summary>
    /// Provides operations for a data input feed.
    /// </summary>
    /// <typeparam name="TSerializable">The type of data that is being sent.</typeparam>
    public interface ISendFeed<in TSerializable>
        where TSerializable : ISerializable
    {
        /// <summary>
        /// Pushes the provided data into the feed.
        /// </summary>
        /// <param name="data">The data to push.</param>
        void Push(TSerializable data);
    }
}
