using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using Landfall.Network;

namespace CitrusLib.Patches
{


    [HarmonyPatch(typeof(ChatMessageCommand), nameof(ChatMessageCommand.Run))]
    public class ChatPatch
    {
        static bool Prefix(byte[] msgData, ServerClient world, byte sender)
        {

            TABGPlayerServer player = Citrus.World.GameRoomReference.FindPlayer(sender);
            if (player == null)
            {
                return false;
            }


            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    byte index = binaryReader.ReadByte();
                    byte count = binaryReader.ReadByte();
                    string message = Encoding.Unicode.GetString(binaryReader.ReadBytes((int)count));

                    string[] prms = message.Split(' ');

                    int i = 0;
                    foreach (string s in prms)
                    {
                        prms[i] = s.ToLower();
                        i++;
                    }


                    if (prms[0].StartsWith("/"))
                    {
                        //runs command and doesnt say it in chat
                        List<string> prmsReal = prms.ToList();
                        prmsReal.RemoveAt(0);
                        Citrus.RunCommand(prms[0].Replace("/", ""), prmsReal.ToArray(), player);
                        return true;
                    }
                }
            }

            return true;
        }
    }
}