using System.Collections.Generic;
using Landfall.Network;

namespace CitrusLib
{
    public static partial class Citrus
    {
        // ─── Weapon Patch Dictionaries ────────────────────────────────────────────
        // All stored by weapon ID. Unset values fall back to the game's defaults.

        private static Dictionary<int, float> weaponDamageMultipliers      = new Dictionary<int, float>();
        private static Dictionary<int, float> weaponSpreadMultipliers       = new Dictionary<int, float>();
        private static Dictionary<int, float> weaponHipSpreadValues         = new Dictionary<int, float>();
        private static Dictionary<int, float> weaponExtraSpreads            = new Dictionary<int, float>();
        private static Dictionary<int, float> weaponFireRateMultipliers     = new Dictionary<int, float>();
        private static Dictionary<int, float> weaponBulletSpeedMultipliers  = new Dictionary<int, float>();
        private static Dictionary<int, int>   weaponMagSizeOverrides        = new Dictionary<int, int>();
        private static Dictionary<int, float> weaponReloadTimeMultipliers   = new Dictionary<int, float>();

        // Current Player Weapons: PlayerIndex -> CurrentWeaponID
        public static Dictionary<byte, int> PlayerActiveWeapons = new Dictionary<byte, int>();


        // ─── Setters ──────────────────────────────────────────────────────────────

        public static void SetWeaponDamageMultiplier(int id, float v)      => weaponDamageMultipliers[id]      = v;
        public static void SetWeaponSpreadMultiplier(int id, float v)      => weaponSpreadMultipliers[id]      = v;
        public static void SetWeaponHipSpreadValue(int id, float v)        => weaponHipSpreadValues[id]        = v;
        public static void SetWeaponExtraSpread(int id, float v)           => weaponExtraSpreads[id]           = v;
        public static void SetWeaponFireRateMultiplier(int id, float v)    => weaponFireRateMultipliers[id]    = v;
        public static void SetWeaponBulletSpeedMultiplier(int id, float v) => weaponBulletSpeedMultipliers[id] = v;
        public static void SetWeaponMagSizeOverride(int id, int v)         => weaponMagSizeOverrides[id]       = v;
        public static void SetWeaponReloadTimeMultiplier(int id, float v)  => weaponReloadTimeMultipliers[id]  = v;

        /// <summary>Clears all patches for a specific weapon ID.</summary>
        public static void ClearWeaponPatches(int id)
        {
            weaponDamageMultipliers.Remove(id);
            weaponSpreadMultipliers.Remove(id);
            weaponHipSpreadValues.Remove(id);
            weaponExtraSpreads.Remove(id);
            weaponFireRateMultipliers.Remove(id);
            weaponBulletSpeedMultipliers.Remove(id);
            weaponMagSizeOverrides.Remove(id);
            weaponReloadTimeMultipliers.Remove(id);
        }

        /// <summary>Clears every weapon patch — use before a config reload.</summary>
        public static void ClearAllWeaponPatches()
        {
            weaponDamageMultipliers.Clear();
            weaponSpreadMultipliers.Clear();
            weaponHipSpreadValues.Clear();
            weaponExtraSpreads.Clear();
            weaponFireRateMultipliers.Clear();
            weaponBulletSpeedMultipliers.Clear();
            weaponMagSizeOverrides.Clear();
            weaponReloadTimeMultipliers.Clear();
        }


        // ─── Getters (with game-default fallback) ─────────────────────────────────
        // Multipliers default to 1.0 (no change).

        public static float GetWeaponDamageMultiplier(int id)      => weaponDamageMultipliers.TryGetValue(id, out var v)      ? v : 1f;
        public static float GetWeaponSpreadMultiplier(int id)      => weaponSpreadMultipliers.TryGetValue(id, out var v)      ? v : 1f;
        public static float GetWeaponFireRateMultiplier(int id)    => weaponFireRateMultipliers.TryGetValue(id, out var v)    ? v : 1f;
        public static float GetWeaponBulletSpeedMultiplier(int id) => weaponBulletSpeedMultipliers.TryGetValue(id, out var v) ? v : 1f;
        public static float GetWeaponReloadTimeMultiplier(int id)  => weaponReloadTimeMultipliers.TryGetValue(id, out var v)  ? v : 1f;

        // HipSpread, ExtraSpread and MagSize are direct overrides, not multipliers —
        // only applied when a patch is explicitly set.
        public static bool TryGetWeaponHipSpreadValue(int id, out float v) => weaponHipSpreadValues.TryGetValue(id, out v);
        public static bool TryGetWeaponExtraSpread(int id, out float v)    => weaponExtraSpreads.TryGetValue(id, out v);
        public static bool TryGetWeaponMagSizeOverride(int id, out int v)  => weaponMagSizeOverrides.TryGetValue(id, out v);


        // ─── Player Weapon Lookup ─────────────────────────────────────────────────

        /// <summary>
        /// Gets the current weapon ID directly from the player's equipment slots.
        /// Returns -1 if no weapon is active.
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