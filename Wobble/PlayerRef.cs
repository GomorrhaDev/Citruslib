using System.Collections.Generic;
using Landfall.Network;

namespace WobbleBridge
{
    /// <summary>
    /// Wraps a TABGPlayerServer with a flexible key-value data store.
    /// Can be used by other mods to attach custom data to a player.
    /// </summary>
    public class PlayerRef
    {
        public Dictionary<string, object> data;
        public TABGPlayerServer player;

        public PlayerRef(TABGPlayerServer player)
        {
            this.player = player;
            data = new Dictionary<string, object>
            {
                { "originalTeam", player.GroupIndex },
                { "aliveAware",  !player.IsDead     }
            };
        }

        /// <summary>
        /// Retrieves a typed value from the data store. Returns default(T) if the key doesn't exist.
        /// </summary>
        public T Get<T>(string key)
        {
            if (data.TryGetValue(key, out object ret))
                return (T)ret;
            return default;
        }

        public static explicit operator TABGPlayerServer(PlayerRef pref)
        {
            if (pref == null) return null;
            return pref.player;
        }

        // Two PlayerRefs are equal if they wrap the same player index.
        // Allows List.Distinct() and similar LINQ operations to work correctly.
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return ((PlayerRef)obj).player.PlayerIndex == this.player.PlayerIndex;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Represents a team group with a list of PlayerRefs and a group index.
    /// </summary>
    public class PlayerTeam
    {
        public List<PlayerRef> players = new List<PlayerRef>();
        public byte groupIndex;

        public PlayerTeam(byte ind)
        {
            players    = new List<PlayerRef>();
            groupIndex = ind;
        }
    }
}