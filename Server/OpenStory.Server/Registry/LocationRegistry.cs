using System;
using System.Collections.Generic;
using System.Linq;
using OpenStory.Framework.Model.Common;

namespace OpenStory.Server.Registry
{
    /// <summary>
    /// A registry for player locations.
    /// </summary>
    public sealed class LocationRegistry : ILocationRegistry
    {
        private readonly Dictionary<CharacterKey, PlayerLocation> locations;

        /// <summary>
        /// Initializes a new instance of <see cref="LocationRegistry"/>.
        /// </summary>
        public LocationRegistry()
        {
            this.locations = new Dictionary<CharacterKey, PlayerLocation>();
        }

        /// <inheritdoc />
        public Dictionary<CharacterKey, PlayerLocation> GetLocationsForAll(IEnumerable<CharacterKey> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            return keys
                .Distinct()
                .ToDictionary(key => key, this.GetLocation);
        }

        /// <inheritdoc />
        public PlayerLocation GetLocation(CharacterKey key)
        {
            PlayerLocation location;
            this.locations.TryGetValue(key, out location);
            return location;
        }

        /// <inheritdoc />
        public void SetLocation(CharacterKey key, int channelId, int mapId)
        {
            var location = new PlayerLocation(channelId, mapId);
            if (this.locations.ContainsKey(key))
            {
                this.locations[key] = location;
            }
            else
            {
                this.locations.Add(key, location);
            }
        }

        /// <inheritdoc />
        public void RemoveLocation(CharacterKey key)
        {
            this.locations.Remove(key);
        }
    }
}
