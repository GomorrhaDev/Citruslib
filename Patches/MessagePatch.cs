using System;
using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{



//reduces multiple-recipient packages into seperate ones before trying to send to prevent package duplication
//... and to make my bot mod work more seamlessly once i make it

    [HarmonyPatch(typeof(ServerClient), nameof(ServerClient.SendMessageToClients),
        new Type[] { typeof(EventCode), typeof(byte[]), typeof(byte[]), typeof(bool), typeof(bool) })]
    public class MessagePatch
    {

        static bool Prefix(EventCode opCode, byte[] buffer, byte[] recipents, bool reliable, bool alsoSendToTeamates)
        {
            if (recipents.Length != 1)
            {
                foreach (byte b in recipents)
                {
                    Wobble.World.SendMessageToClients(opCode, buffer, b, reliable, false); //haha! saved the world!
                }

                return false;
            }
            else
            {
                if (recipents.Length == 0)
                {
                    Wobble.log.LogError("Sending message to no recipient???");
                    return false;
                }

                if (recipents[0] == byte.MaxValue)
                {
                    foreach (TABGPlayerServer p in Wobble.World.GameRoomReference.Players)
                    {
                        Wobble.World.SendMessageToClients(opCode, buffer, p.PlayerIndex, reliable, false);
                    }

                    return false;
                }

            }

            return true;

        }

    }
}