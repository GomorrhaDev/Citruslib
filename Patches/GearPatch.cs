using System.IO;
using HarmonyLib;
using Landfall.Network;

namespace CitrusLib.Patches
{
    
//prevents players from setting their gear every time they spawn. allows opposing-clientsided gear changing
    [HarmonyPatch(typeof(GearChangeCommand), nameof(GearChangeCommand.Run))]
    public class GearPatch
    {
        static bool Prefix(byte[] msgData, ServerClient world)
        {
            byte index;
            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    index = binaryReader.ReadByte();
                    /*
                    int num = binaryReader.ReadInt32();
                    array = new int[num];
                    for (int i = 0; i < num; i++)
                    {
                        array[i] = binaryReader.ReadInt32();
                    }*/
                }
            }

            TABGPlayerServer tabgplayerServer = world.GameRoomReference.FindPlayer(index);
            if (tabgplayerServer == null || tabgplayerServer.GearData.Length != 0)
            {
                return false;
            }

            return true;

        }
    }
}