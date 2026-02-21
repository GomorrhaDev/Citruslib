using System.Collections.Generic;
using System.Linq;
using Landfall.Network;

namespace CitrusLib
{
    public static partial class Citrus
    {
        /// <summary>
        /// Searches for a player first by name prefix, then by player index.
        /// Good for chat commands where users can type either a name or a numeric ID.
        /// Returns false if none or more than one match is found.
        /// </summary>
        public static bool PlayerChatSearch(string name, out TABGPlayerServer result)
        {
            result = null;
            if (PlayerWithName(name, out result)) return true;

            if (byte.TryParse(name, out byte ind))
                return PlayerWithIndex(ind, out result);

            return false;
        }

        /// <summary>
        /// Finds a player by their numeric player index.
        /// Returns false if none or more than one match is found.
        /// </summary>
        public static bool PlayerWithIndex(byte index, out TABGPlayerServer result)
        {
            result = null;

            List<TABGPlayerServer> matches = World.GameRoomReference.Players
                .Where(x => x != null)
                .Where(p => p.PlayerIndex == index)
                .ToList();

            if (matches.Count != 0) result = matches.First();

            return matches.Count == 1;
        }

        /// <summary>
        /// Finds a player whose name starts with the provided string (case-insensitive).
        /// Returns false if none or more than one match is found.
        /// </summary>
        public static bool PlayerWithName(string name, out TABGPlayerServer result)
        {
            result = null;

            List<TABGPlayerServer> matches = World.GameRoomReference.Players
                .Where(x => x != null)
                .Where(p => p.PlayerName.ToLower().StartsWith(name))
                .ToList();

            if (matches.Count != 0) result = matches.First();

            return matches.Count == 1;
        }
    }
}