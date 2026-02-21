using System.Collections.Generic;
using Landfall.Network;
using Unity.Networking.Transport;

namespace CitrusLib
{
    public static partial class Citrus
    {
        static ServerClient currentWorld = null;

        public static ServerClient World
        {
            get
            {
                if (currentWorld != null) return currentWorld;
                currentWorld = UnityEngine.Object.FindObjectOfType<ServerClient>();
                return currentWorld;
            }
        }

        public static NetworkDriver Network;

        public static Queue<byte> kicklist = new Queue<byte>();

        public static List<PlayerRef> players = new List<PlayerRef>();

        public static List<PlayerTeam> teams = new List<PlayerTeam>();

        public static List<UnityTransportServer.BufferedPackage> queue = new List<UnityTransportServer.BufferedPackage>();

        public static Dictionary<byte, Queue<UnityTransportServer.BufferedPackage>> buffQueue = new Dictionary<byte, Queue<UnityTransportServer.BufferedPackage>>();
    }
}