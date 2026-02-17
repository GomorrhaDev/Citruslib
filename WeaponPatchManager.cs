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
        private static string ConfigPath
        {
            get
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
                return Path.Combine(directoryInfo.Parent.FullName, "weapon_patches.json");
            }
        }
        private static Dictionary<int, float> customWeaponMultipliers = new Dictionary<int, float>();

        public static void Initialize()
        {
            string oldPath = Path.Combine(Paths.ConfigPath, "weapon_patches.json");
            string newPath = ConfigPath;

            if (File.Exists(oldPath) && !File.Exists(newPath))
            {
                try
                {
                    File.Move(oldPath, newPath);
                    Citrus.log.Log($"WeaponPatchManager: Moved config from {oldPath} to {newPath}");
                }
                catch (Exception e)
                {
                    Citrus.log.LogError($"WeaponPatchManager: Failed to move old config: {e.Message}");
                }
            }

            LoadConfig();
        }

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

                customWeaponMultipliers.Clear();

                if (config?.Patches != null)
                {
                    foreach (var patch in config.Patches)
                    {
                        int? id = null;

                        // Check if it's an ID
                        if (int.TryParse(patch.Key, out int parsedId))
                        {
                            id = parsedId;
                        }
                        else
                        {
                            // Try to map name
                            if (Enum.TryParse<Item>(patch.Key, true, out Item item))
                            {
                                id = (int)item;
                            }
                            else if (Enum.TryParse<Item>("_" + patch.Key, true, out Item itemWithUnderscore))
                            {
                                id = (int)itemWithUnderscore;
                            }
                        }

                        if (id.HasValue)
                        {
                            customWeaponMultipliers[id.Value] = patch.Value;
                            Citrus.SetWeaponDamageMultiplier(id.Value, patch.Value);
                        }
                        else
                        {
                            Citrus.log.Log($"WeaponPatchManager: Could not find weapon ID for '{patch.Key}'");
                        }
                    }
                }
                
                Citrus.log.Log($"WeaponPatchManager: Loaded {customWeaponMultipliers.Count} weapon patches.");
            }
            catch (Exception e)
            {
                Citrus.log.LogError($"WeaponPatchManager: Error loading config: {e.Message}");
            }
        }

        public static void ReloadConfig()
        {
            LoadConfig();
        }

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new WeaponConfig
            {
                Patches = new Dictionary<string, float>
                {
                    { "MG42", 0.8f },
                    { "153", 1.5f } // Example using ID for AUG
                }
            };

            try
            {
                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Citrus.log.Log("WeaponPatchManager: Created default weapon_patches.json");
                
                // Also apply these defaults
                LoadConfig();
            }
            catch (Exception e)
            {
                Citrus.log.LogError($"WeaponPatchManager: Error creating default config: {e.Message}");
            }
        }

        public class WeaponConfig
        {
            public Dictionary<string, float> Patches { get; set; }
        }
    }
}
