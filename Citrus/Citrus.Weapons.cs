using System.Collections.Generic;
using Landfall.Network;

namespace CitrusLib
{
    public static partial class Citrus
    {
        // Weapon Damage Multipliers: WepID -> Multiplier
        private static Dictionary<int, float> weaponDamageMultipliers = new Dictionary<int, float>();

        // Current Player Weapons: PlayerIndex -> CurrentWeaponID
        public static Dictionary<byte, int> PlayerActiveWeapons = new Dictionary<byte, int>();

        /// <summary>
        /// Sets a damage multiplier for a specific weapon ID.
        /// </summary>
        public static void SetWeaponDamageMultiplier(int weaponId, float multiplier)
        {
            if (weaponDamageMultipliers.ContainsKey(weaponId))
                weaponDamageMultipliers[weaponId] = multiplier;
            else
                weaponDamageMultipliers.Add(weaponId, multiplier);
        }

        /// <summary>
        /// Returns the damage multiplier for a weapon ID, defaulting to 1.0 if none is set.
        /// </summary>
        public static float GetWeaponDamageMultiplier(int weaponId)
        {
            if (weaponDamageMultipliers.TryGetValue(weaponId, out float mult))
                return mult;
            return 1.0f;
        }

        /// <summary>
        /// Gets the current weapon ID directly from the player's equipment slots.
        /// Returns -1 if no weapon is found.
        /// </summary>
        public static int GetCurrentWeaponID(TABGPlayerServer player)
        {
            if (player == null || player.Equipment == null || player.Equipment.Length < 6) return -1;

            int slotFlag = (int)player.Equipment[5];
            if (slotFlag == 1) return (int)player.Equipment[0];
            if (slotFlag == 2) return (int)player.Equipment[2];
            if (slotFlag == 3) return (int)player.Equipment[4];

            return -1;
        }
    }
}