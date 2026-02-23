using System.Collections.Generic;
using System.IO;
using Epic.OnlineServices.Auth;
using HarmonyLib;
using Landfall.Network;
using Unity.Collections;
using Unity.Networking.Transport;

namespace WobbleBridge.Patches
{
    // required for kicking players properly. dont ask why.
    [HarmonyPatch(typeof(PlayerUpdateCommand), nameof(PlayerUpdateCommand.Run))]
    class PlayerUpdatePatch
    {
        static void Prefix(byte[] msgData, ServerClient world)
        {
            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    byte ind = binaryReader.ReadByte();

                    PlayerRef p = Wobble.players.Find(pr => pr.player.PlayerIndex == ind);

                    if (p == null)
                    {
                        return; // how
                    }

                    if (!p.player.IsDead)
                    {
                        p.data["aliveAware"] = true;
                    }
                }
            }
        }
    }

    // required for kicking players properly. dont ask why.
    [HarmonyPatch(typeof(UnityTransportServer), "Update")]
    class UpdatePatch
    {
        static void Prefix(ref BidirectionalDictionary<byte, NetworkConnection> ___m_playerIDToConnection, UnityTransportServer __instance, ref NativeList<NetworkConnection> ___m_connections)
        {
            // apparent naitivelists are special and cannot be compared to null?
            if (Wobble.kicklist != null & ___m_playerIDToConnection != null)
            {
                while (Wobble.kicklist.Count != 0)
                {
                    byte pb = Wobble.kicklist.Dequeue();
                    NetworkConnection nc;
                    if (!___m_playerIDToConnection.TryGetValue(pb, out nc))
                    {
                        Wobble.log.LogError(string.Format("Failed to find connection fo player ID: {0}", pb));
                        continue;
                    }
                    // please work please work please work
                    int j = ___m_connections.IndexOf(nc);
                    ___m_playerIDToConnection.Remove(___m_connections[j]);
                    Wobble.log.Log(string.Format("Client: {0} disconnected from server", ___m_connections[j].InternalId));
                    ___m_connections[j] = default(NetworkConnection);
                    TABGPlayerServer tabgplayerServer = Wobble.World.GameRoomReference.FindPlayer(pb);
                    if (tabgplayerServer != null)
                    {
                        Wobble.World.HandlePlayerLeave(tabgplayerServer);
                    }
                }
            }
        }

        static void Postfix(ref BidirectionalDictionary<byte, NetworkConnection> ___m_playerIDToConnection, ref NativeList<NetworkConnection> ___m_connections)
        {
        }
    }

    [HarmonyPatch(typeof(RoomInitRequestCommand), "OnVerifiesEpicToken")]
    class VerifyPatch
    {
        static void Postfix(ref VerifyIdTokenCallbackInfo data)
        {
            TABGPlayerServer tabgplayerServer = data.ClientData as TABGPlayerServer;

            if (tabgplayerServer != null & tabgplayerServer.EpicUserName != null)
            {
                GuestBook.SignGuestBook(tabgplayerServer);
            }
        }
    }

    // creates player references and team references
    [HarmonyPatch(typeof(GameRoom), nameof(GameRoom.AddPlayer))]
    class AddPlayerPatch
    {
        static void Postfix(TABGPlayerServer p, bool wantsToBeAlone)
        {
            if (Wobble.players.Find(pr => pr.player.PlayerIndex == p.PlayerIndex) != null)
            {
                return;
            }

            PlayerRef pRef = new PlayerRef(p);

            if (Wobble.players == null)
            {
                Wobble.log.Log("player list is null?");
                Wobble.players = new List<PlayerRef>();
            }

            Wobble.players.Add(pRef);

            PlayerTeam myTeam = Wobble.teams.Find(t => t.groupIndex == p.GroupIndex);

            if (myTeam == null)
            {
                myTeam = new PlayerTeam(p.GroupIndex);
                Wobble.teams.Add(myTeam);
            }
            myTeam.players.Add(pRef);
        }
    }

    // removes player (but not team reference)
    [HarmonyPatch(typeof(GameRoom), nameof(GameRoom.RemovePlayer))]
    class RemovePlayerPatch
    {
        static void Prefix(TABGPlayerServer p)
        {
            if (p == null)
            {
                return;
            }

            PlayerRef player = Wobble.players.Find(pl => pl.player == p);

            if (player == null) return;

            PlayerTeam team = Wobble.teams.Find(t => t.players.Contains(player));

            if (team != null)
            {
                team.players.Remove(player);
                return;
            }
            Wobble.players.Remove(player);
        }
    }
}
