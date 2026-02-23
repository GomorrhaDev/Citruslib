using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{
    [HarmonyPatch(typeof(ChatMessageCommand), nameof(ChatMessageCommand.Run))]
    public class ChatPatch
    {
        static bool Prefix(byte[] msgData, ServerClient world, byte sender)
        {

            TABGPlayerServer player = Wobble.World.GameRoomReference.FindPlayer(sender);

            if (player == null)
            {
                Wobble.log.LogWarning($"[ChatPatch] Player not found for sender index {sender} — aborting.");
                return false;
            }
            

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(msgData))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        byte index = binaryReader.ReadByte();
                        byte count = binaryReader.ReadByte();
                        string message = Encoding.Unicode.GetString(binaryReader.ReadBytes((int)count));
                        
                        string[] prms = message.Split(' ');

                        for (int i = 0; i < prms.Length; i++)
                            prms[i] = prms[i].ToLower();
                        

                        if (prms[0].StartsWith("/"))
                        {
                            string commandName = prms[0].Replace("/", "");

                            List<string> prmsReal = prms.ToList();
                            prmsReal.RemoveAt(0);
                            
                            Wobble.RunCommand(commandName, prmsReal.ToArray(), player);
                            
                            return true;
                        }
                        else
                        {
                            Wobble.log.Log($"[ChatPatch] Not a command, passing through to original.");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Wobble.log.LogError($"[ChatPatch] Exception during message parsing: {e.Message}\n{e.StackTrace}");
            }

            return true;
        }
    }
}