using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BepInEx;
using UnityEngine;

namespace CitrusLib
{
    public class WeaponPatchManager
    {
        // ─── Config Path ──────────────────────────────────────────────────────────

        private static string ConfigPath
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
                return Path.Combine(dir.Parent.FullName, "weapon_patches.json");
            }
        }


        // ─── Lifecycle ────────────────────────────────────────────────────────────

        public static void Initialize()
        {
            // Migrate old config location if needed
            string oldPath = Path.Combine(Paths.ConfigPath, "weapon_patches.json");
            if (File.Exists(oldPath) && !File.Exists(ConfigPath))
            {
                try
                {
                    File.Move(oldPath, ConfigPath);
                    Citrus.log.Log($"WeaponPatchManager: Moved config from {oldPath} to {ConfigPath}");
                }
                catch (Exception e)
                {
                    Citrus.log.LogError($"WeaponPatchManager: Failed to move old config: {e.Message}");
                }
            }

            LoadConfig();
        }

        public static void ReloadConfig()
        {
            Citrus.ClearAllWeaponPatches();
            LoadConfig();
        }


        // ─── Load ─────────────────────────────────────────────────────────────────

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                CreateDefaultConfig();
                return;
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                var config = JsonConvert.DeserializeObject<WeaponConfig>(json);

                if (config?.Patches == null)
                {
                    Citrus.log.LogWarning("WeaponPatchManager: Config file is empty or invalid.");
                    return;
                }

                int loaded = 0;

                foreach (var kvp in config.Patches)
                {
                    if (!TryResolveWeaponId(kvp.Key, out int id))
                    {
                        Citrus.log.LogWarning($"WeaponPatchManager: Could not resolve weapon ID for '{kvp.Key}' — skipping.");
                        continue;
                    }

                    ApplyPatch(id, kvp.Value);
                    loaded++;
                }

                Citrus.log.Log($"WeaponPatchManager: Loaded {loaded} weapon patch(es).");
            }
            catch (Exception e)
            {
                Citrus.log.LogError($"WeaponPatchManager: Error loading config: {e.Message}");
            }
        }


        // ─── Apply ────────────────────────────────────────────────────────────────

        private static void ApplyPatch(int id, WeaponPatch patch)
        {
            if (patch == null) return;

            if (patch.DamageMultiplier.HasValue)
                Citrus.SetWeaponDamageMultiplier(id, patch.DamageMultiplier.Value);

            if (patch.SpreadMultiplier.HasValue)
                Citrus.SetWeaponSpreadMultiplier(id, patch.SpreadMultiplier.Value);

            if (patch.HipSpreadValue.HasValue)
                Citrus.SetWeaponHipSpreadValue(id, patch.HipSpreadValue.Value);

            if (patch.ExtraSpread.HasValue)
                Citrus.SetWeaponExtraSpread(id, patch.ExtraSpread.Value);

            if (patch.FireRateMultiplier.HasValue)
                Citrus.SetWeaponFireRateMultiplier(id, patch.FireRateMultiplier.Value);

            if (patch.BulletSpeedMultiplier.HasValue)
                Citrus.SetWeaponBulletSpeedMultiplier(id, patch.BulletSpeedMultiplier.Value);

            if (patch.MagSizeOverride.HasValue)
                Citrus.SetWeaponMagSizeOverride(id, patch.MagSizeOverride.Value);

            if (patch.ReloadTimeMultiplier.HasValue)
                Citrus.SetWeaponReloadTimeMultiplier(id, patch.ReloadTimeMultiplier.Value);
        }


        // ─── Helpers ──────────────────────────────────────────────────────────────

        private static bool TryResolveWeaponId(string key, out int id)
        {
            id = 0;
            if (int.TryParse(key, out id)) return true;
            if (Enum.TryParse<Item>(key, true, out Item item))       { id = (int)item; return true; }
            if (Enum.TryParse<Item>("_" + key, true, out Item item2)) { id = (int)item2; return true; }
            return false;
        }


        // ─── Default Config ───────────────────────────────────────────────────────

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new WeaponConfig
            {
                Patches = new Dictionary<string, WeaponPatch>
                {
                    ["MG42"] = new WeaponPatch
                    {
                        DamageMultiplier   = 0.8f,
                        SpreadMultiplier   = 1.2f,
                        FireRateMultiplier = 0.9f
                    },
                    ["153"] = new WeaponPatch   // AUG by ID
                    {
                        DamageMultiplier      = 1.5f,
                        BulletSpeedMultiplier = 1.2f,
                        MagSizeOverride       = 40
                    }
                }
            };

            try
            {
                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Citrus.log.Log("WeaponPatchManager: Created default weapon_patches.json");
                LoadConfig();
            }
            catch (Exception e)
            {
                Citrus.log.LogError($"WeaponPatchManager: Error creating default config: {e.Message}");
            }
        }


        // ─── Config Model ─────────────────────────────────────────────────────────

        public class WeaponConfig
        {
            public Dictionary<string, WeaponPatch> Patches { get; set; }
        }

        /// <summary>
        /// All fields are nullable — only set fields will override the game's defaults.
        /// </summary>
        public class WeaponPatch
        {
            /// <summary>Multiplier applied to all damage dealt by this weapon. (1.0 = no change)</summary>
            public float? DamageMultiplier { get; set; }

            /// <summary>Multiplier applied to the weapon's base spread. (1.0 = no change)</summary>
            public float? SpreadMultiplier { get; set; }

            /// <summary>
            /// Overrides the hip-fire spread value directly.
            /// The game default varies per weapon — set to 0 for perfect hip-fire accuracy.
            /// </summary>
            public float? HipSpreadValue { get; set; }

            /// <summary>
            /// Overrides the extra flat spread added on top of the base spread.
            /// Set to 0 to remove all extra spread.
            /// </summary>
            public float? ExtraSpread { get; set; }

            /// <summary>Multiplier applied to fire rate. (>1.0 = faster, &lt;1.0 = slower)</summary>
            public float? FireRateMultiplier { get; set; }

            /// <summary>Multiplier applied to bullet travel speed. (1.0 = no change)</summary>
            public float? BulletSpeedMultiplier { get; set; }

            /// <summary>
            /// Directly overrides the magazine size.
            /// Replaces the game value entirely — not a multiplier.
            /// </summary>
            public int? MagSizeOverride { get; set; }

            /// <summary>Multiplier applied to reload time. (&lt;1.0 = faster reload, >1.0 = slower)</summary>
            public float? ReloadTimeMultiplier { get; set; }
        }
    }
}