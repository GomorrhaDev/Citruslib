using System;
using System.IO;
using HarmonyLib;
using Landfall.Network;

namespace WobbleBridge.Patches
{

    [HarmonyPatch(typeof(WeaponChangedCommand), nameof(WeaponChangedCommand.Run))]
    public class WeaponChangedPatch
    {
        static void Prefix(byte[] msgData)
        {
            try
            {
                using (MemoryStream input = new MemoryStream(msgData))
                {
                    using (BinaryReader binaryReader = new BinaryReader(input))
                    {
                        byte playerIndex = binaryReader.ReadByte();
                        byte slotFlag = binaryReader.ReadByte();
                        short w1A = binaryReader.ReadInt16(); // Slot 1
                        short w1B = binaryReader.ReadInt16(); // Slot 1 (dual)
                        short w2A = binaryReader.ReadInt16(); // Slot 2
                        short w2B = binaryReader.ReadInt16(); // Slot 2 (dual)
                        short w3A = binaryReader.ReadInt16(); // Slot 3

                        int currentWeapon = -1;
                        // slotFlag defines which slot is currently active
                        if (slotFlag == 0) currentWeapon = w1A;
                        else if (slotFlag == 1) currentWeapon = w2A;
                        else if (slotFlag == 2) currentWeapon = w3A;

                        if (Wobble.PlayerActiveWeapons.ContainsKey(playerIndex))
                            Wobble.PlayerActiveWeapons[playerIndex] = currentWeapon;
                        else
                            Wobble.PlayerActiveWeapons.Add(playerIndex, currentWeapon);
                    }
                }
            }
            catch (Exception e)
            {
                Wobble.log.LogError("Error in WeaponChangedPatch: " + e.Message);
            }
        }
    }
}